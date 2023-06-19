using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private Transform upPartMove; // Player_Model scene name
    [SerializeField]
    private Transform upPartIdle; // Player Spine
    [SerializeField]
    private Transform whole;      // RotationBuffer scene name


    [SerializeField]
    private Transform sword;

    [SerializeField]
    private Transform gun;


    [SerializeField]
    private Animator anim;

    [SerializeField]
    private GameObject fatalityImage;

    [SerializeField]
    private Enemy enemyNear;
    private Transform enemyBack;

    private bool isEnemyNearBy;
    private bool isFinishing;

    private Vector3 inputMoveDir; // stored in local space
    private bool isMoving;
    private void Awake()
    {
        isEnemyNearBy = false; 
        isFinishing = false;
        isMoving = false;
        inputMoveDir = new Vector3(0, 0, 1); // KeyCode.W inputMoveDir
    }

    private void OnTriggerEnter(Collider other)
    {
        enemyNear = other.gameObject.GetComponent<Enemy>();
        if (enemyNear != null)
        {
            isEnemyNearBy = true;
            enemyBack = enemyNear.GetBackTransform();
            fatalityImage.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Enemy>() != null)
        {
            isEnemyNearBy = false;
            enemyBack = null;
            enemyNear = null;
            fatalityImage.SetActive(false);
        }
    }

    public void Update()
    {
        if (isFinishing)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space) && isEnemyNearBy)
        {

            fatalityImage.SetActive(false);
            StartCoroutine(MoveToEnemy());
            isFinishing = true;
            return;
        }


        Vector3 inputMoveDirTemp = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            inputMoveDirTemp += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputMoveDirTemp += -Vector3.right;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputMoveDirTemp += -Vector3.forward;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputMoveDirTemp += Vector3.right;
        }



        if (inputMoveDirTemp == Vector3.zero)
        {
            isMoving = false;
            anim.SetBool("isRunning", false);


/*            if (!isRotatingBigPath)
            { */
                upPartMove.localRotation = Quaternion.identity; 
            //}
            
            
            
            return;
        }
        inputMoveDirTemp.Normalize();
        inputMoveDir = inputMoveDirTemp;


        isMoving = true;
        anim.SetBool("isRunning", true);
        Vector3 translation = inputMoveDir * moveSpeed * Time.deltaTime;
        transform.Translate(translation, Space.Self);
        RotateUpPartMoving();
        RotateToMoveInput();
    }

    private IEnumerator MoveToEnemy()
    {
        inputMoveDir = enemyBack.forward;
        inputMoveDir = transform.InverseTransformDirection(inputMoveDir);

        sword.gameObject.SetActive(true);
        gun.gameObject.SetActive(false);

        Vector3 totalTranslation = enemyBack.position - transform.position;
        float halfSecond = 0.5f;
        while (halfSecond > 0)
        {
            float deltaFraction = Time.deltaTime * (1 / 0.5f);
            transform.Translate(totalTranslation * deltaFraction, Space.World);
            whole.rotation = Quaternion.RotateTowards
                (whole.rotation, enemyBack.rotation, 180 * deltaFraction);
            upPartMove.localRotation = Quaternion.RotateTowards
                (upPartMove.localRotation, Quaternion.identity, 180 * deltaFraction);
            upPartIdle.localRotation = Quaternion.RotateTowards
                (upPartIdle.localRotation, Quaternion.identity, 180 * deltaFraction);
            halfSecond -= Time.deltaTime;
            yield return null;
        }
        anim.SetTrigger("finishHim");
        enemyNear.SetFinishing();
        yield return new WaitForSeconds(2.5f);
        anim.SetTrigger("finishHim");
        yield return new WaitForSeconds(0.5f);
        sword.gameObject.SetActive(false);
        gun.gameObject.SetActive(true);
        isFinishing = false;
    }


    private void RotateToMoveInput()
    {
        Quaternion targetRot = Quaternion.LookRotation(inputMoveDir, Vector3.up);
        Quaternion fromRot = whole.localRotation;
        whole.localRotation = Quaternion.RotateTowards(fromRot, targetRot, 180 * Time.deltaTime);
    }


    private float prevSignMoving;
    private bool isRotatingBigPath = false;
    private void RotateUpPartMoving()
    {
        if (isRotatingBigPath)
        {
            return;
        }
        Ray mouseRay = Camera.main.ScreenPointToRay
            (new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        RaycastHit rayHit;
        if (Physics.Raycast(mouseRay, out rayHit, Mathf.Infinity,
            LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
        {
            Vector3 mousePos = rayHit.point;
            Vector3 mouseDir = (mousePos - transform.position);
            mouseDir.Normalize();
            Vector3 worldInputMoveDir = transform.TransformDirection(inputMoveDir);

            float sign = Mathf.Sign(Vector3.Cross(worldInputMoveDir, mouseDir).y);
            float cos = Mathf.Clamp(Vector3.Dot(mouseDir, worldInputMoveDir), -1, 1);
            float degrees = Mathf.Acos(cos) * Mathf.Rad2Deg * sign;
            degrees = Mathf.Clamp(degrees, -100, 100);


                        Debug.DrawRay(transform.position + new Vector3(0, 1, 0),
                                mouseDir, Color.red, 0.1f, false);
                        Debug.DrawRay(transform.position + new Vector3(0, 1, 0),
                                worldInputMoveDir * 10, Color.red, 0.1f, false);

            /*            if (prevSignMoving != sign && Mathf.Abs(degrees) > 80)
                        {
                            StartCoroutine(RotateBigPathMoving(degrees));
                        }
                        else
                        {*/
            Quaternion targetRot = Quaternion.AngleAxis(degrees, Vector3.up);
                Quaternion fromRot = upPartMove.localRotation;
                Quaternion rot = Quaternion.RotateTowards(fromRot, targetRot, 10 * 180 * Time.deltaTime);
                upPartMove.localRotation = rot;
                anim.SetFloat("AngleUp", degrees);
            //}
            prevSignMoving = sign;
        }
    }
    private IEnumerator RotateBigPathMoving(float degrees)
    {
        isRotatingBigPath = true; 
        Quaternion targetRot = Quaternion.AngleAxis(0, Vector3.up);
        float angleUp = degrees;
        while (upPartMove.localRotation != targetRot)
        {
            upPartMove.localRotation = Quaternion.RotateTowards
                (upPartMove.localRotation, targetRot, 100 * Time.deltaTime);
            angleUp += -Mathf.Sign(angleUp);
            anim.SetFloat("AngleUp", -angleUp);
            yield return null;
        }
        targetRot = Quaternion.AngleAxis(degrees, Vector3.up);
        while (upPartMove.localRotation != targetRot)
        {
            upPartMove.localRotation = Quaternion.RotateTowards
                (upPartMove.localRotation, targetRot, 100 * Time.deltaTime);
            angleUp += -Mathf.Sign(degrees);
            anim.SetFloat("AngleUp", -angleUp);
            yield return null;
        }
        isRotatingBigPath = false;
    }

    private Vector3 currentEuler;
    private bool isCurrentEulerInit = false;
    private float prevSignIdle;
    bool isRotateToZeroCompleted = false;
    bool isRotateToOppositeCompleted = false;
    float LerpParam;
    float targetDegrees;
    private void LateUpdate()
    {
        if (isMoving)
        {
            return;
        }
        if (isFinishing)
        {
            Vector3 rot = upPartIdle.localEulerAngles;
            rot.x = 0;
            upPartIdle.localEulerAngles = rot;
            return;
        }
        if (!isCurrentEulerInit)
        {
            currentEuler = upPartIdle.localEulerAngles;
            isCurrentEulerInit = true;
        }
        /*if (isRotatingBigPath)
        {
            Vector3 targetRot = currentEuler;

            if (!isRotateToZeroCompleted)
            {
                targetRot.x = 0;
                upPartIdle.localEulerAngles = Vector3.Lerp(currentEuler, targetRot, LerpParam); ;
                LerpParam += 0.01f;
                if (LerpParam > 1)
                {
                    isRotateToZeroCompleted = true;
                    LerpParam = 0;
                    currentEuler.x = 0;
                }
            }
            else if (!isRotateToOppositeCompleted)
            {
                targetRot.x = targetDegrees;
                upPartIdle.localEulerAngles = Vector3.Lerp(currentEuler, targetRot, LerpParam); ;
                LerpParam += 0.01f;
                if (LerpParam > 1)
                {
                    isRotateToOppositeCompleted = true;
                    LerpParam = 0;
                    currentEuler.x = targetDegrees;
                }
            }
            else
            {
                isRotatingBigPath = false;
                isRotateToZeroCompleted = false;
                isRotateToOppositeCompleted = false;
                upPartIdle.localEulerAngles = currentEuler;
            }
            return;
        }*/
        upPartIdle.localEulerAngles = currentEuler;
        Ray ray = Camera.main.ScreenPointToRay(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        RaycastHit rayHit;
        if (Physics.Raycast(ray, out rayHit, Mathf.Infinity, 
            LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
        {
            Vector3 mousePos = rayHit.point;
            Vector3 mouseDir = (mousePos - transform.position);
            mouseDir.Normalize();
            Vector3 worldInputMoveDir = transform.TransformDirection(inputMoveDir);


            float sign = Mathf.Sign(Vector3.Cross(mouseDir, worldInputMoveDir).y);
            float cos = Mathf.Clamp(Vector3.Dot(mouseDir, worldInputMoveDir), -1, 1);
            float degrees = Mathf.Acos(cos) * Mathf.Rad2Deg * sign;
            degrees = Mathf.Clamp(degrees, -100, 100);

            Debug.DrawRay(transform.position + new Vector3(0, 1, 0),
        mouseDir, Color.red, 0.1f, false);
            Debug.DrawRay(transform.position + new Vector3(0, 1, 0),
                    worldInputMoveDir * 10, Color.red, 0.1f, false);

            if (prevSignIdle != sign && Mathf.Abs(degrees) > 80)
            {
               // isRotatingBigPath = true;
                targetDegrees = degrees;
                prevSignIdle = -prevSignIdle;
            }
            else
            {
                currentEuler.x = degrees;
                upPartIdle.localEulerAngles = currentEuler;
            }
            prevSignIdle = sign;
        }
    }

    #region Debugging
    /*                Debug.DrawRay(transform.position + new Vector3(0, 1, 0),
                    dir, Color.red, 0.1f, false);
                Debug.DrawRay(transform.position + new Vector3(0, 1, 0),
                    moveRotation * 10, Color.red, 0.1f, false);*/
    //Debug.Log("moveRotation " + moveRotation.ToString("F5"));
    #endregion
}
