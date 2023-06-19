using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[RequireComponent(typeof(SkeletAnimation))]
public class MoveMobile : MonoBehaviour
{
    private float moveX, moveZ;

    private SkeletAnimation anim;
    private SkeletAnimation.AnimationState currentAnimationState;

    [SerializeField]
    private float boneWeightFactorToSpeed; // should be less than 1
    [SerializeField]
    private float maxSpeed;
    [SerializeField]
    private float minSpeed;


    Vector3 directionOfMove;
    float speedContr;
    private float speedMult;

    private byte boneCount;

    private bool isPickingUp = false;
    private void Start()
    {
        anim = GetComponent<SkeletAnimation>();
        initialForward = transform.forward;
        IsMoveCoroutineRunning = true;
        isPickingUp = false;
        boneCount = 0;
        UpdateBonesWeight(boneCount);
    }

    public void UpdateMoveControl(float speedContrArg, Vector3 directionArg)
    {
        directionOfMove = directionArg;
        speedContr = speedContrArg;
    }
    
    public void UpdateBonesWeight(byte count)
    {
        boneCount = count;
        speedMult = maxSpeed - (boneCount * boneWeightFactorToSpeed);
        if (speedMult < minSpeed)
        {
            speedMult = minSpeed;
        }
        UpdateCurrentAnimationState();
    }

    private void UpdateCurrentAnimationState()
    {
        if (speedMult < maxSpeed / 4)
        {
            currentAnimationState = SkeletAnimation.AnimationState.IdleSlowWalkBlend;
        }
        else if (speedMult < 2 * maxSpeed / 3)
        {
            currentAnimationState = SkeletAnimation.AnimationState.IdleWalkBlend;
        }
        else //if (speedMult <= 3)
        {
            currentAnimationState = SkeletAnimation.AnimationState.IdleRunBlend;
        }
        anim.animationState = currentAnimationState;
    }


    public void SetPickingUp(bool isPickUp)
    {
        isPickingUp = isPickUp;
        if (isPickUp)
        {
            anim.animationState = SkeletAnimation.AnimationState.PickingUp;
        }
        else
        {
            anim.animationState = currentAnimationState;
        }
    }

    #region MoveProcessing

    private IEnumerator moveCoroutine;
    private bool isMoveCoroutineRunning = false;
    private bool IsMoveCoroutineRunning
    {
        get { return isMoveCoroutineRunning; }
        set
        {
            if (value)
            {
                if (!isMoveCoroutineRunning)
                {
                    isMoveCoroutineRunning = true;
                    moveCoroutine = MoveCoroutine();
                    StartCoroutine(moveCoroutine);
                }
            }
            else
            {
                if (isMoveCoroutineRunning)
                {
                    isMoveCoroutineRunning = false;
                    StopCoroutine(moveCoroutine);
                }
            }
        }
    }
    IEnumerator MoveCoroutine()
    {
        while (true)
        {
            if (!isPickingUp)
            { 

                anim.UpdateBlendData(speedContr);
                //Debug.Log("Speed: " + speed);
                if (speedContr >= 0.2f)
                 {
                     Move(directionOfMove, speedContr);
                 }
                 /*
                 if (anim.animationState != SkeletAnimation.AnimationState.Running)
                 {
                     anim.animationState = SkeletAnimation.AnimationState.Running;
                 }
                 else if (anim.animationState != SkeletAnimation.AnimationState.Idle)
                 {
                     anim.animationState = SkeletAnimation.AnimationState.Idle;
                 }*/
            }
            yield return null;
        }

    }

    private Vector3 initialForward;
    private Vector3 rotationDirection;
    bool isRotating = false;
    private void Move(Vector3 direction, float speedInput)
    {
        Vector3 movement = direction * speedInput * speedMult * Time.deltaTime;
        transform.Translate(movement, Space.World);
        //Debug.Log("Moved " + moveX + " " + moveZ);
        if (isRotating == false)
        {
            if (rotationDirection != direction)
            {
                isRotating = true;
                rotationDirection = direction;
                RotateTo(initialForward, rotationDirection, true);
            }
        }
        else
        {
            RotateTo(initialForward, rotationDirection, true);
        }
    }
    void RotateTo(Vector3 from, Vector3 to, bool isGradual = false)
    {
        float angle = Vector3.SignedAngle(from, to, transform.up);

        if (!isGradual)
        {
        }
        Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
        if (isGradual)
        {
            transform.rotation =
            //targetRotation;
                Quaternion.RotateTowards(transform.rotation, targetRotation, 360);
        }
        else
        {
            transform.rotation *= targetRotation;
        }


        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1)
        {
            isRotating = false;
        }
    }
    #endregion
}
