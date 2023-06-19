using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour
{
    [SerializeField]
    private float angularSpeed;
    [SerializeField]
    private float maxSpeed;
    [SerializeField]
    private float acceleration;

    [SerializeField]
    private Transform muzzle;

    private Renderer[] renders;
    private BoxCollider col;
    private Rigidbody rb;

    private AudioSource shootSound;
    private AudioSource moveSound;

    private Vector3 forceToApply;

    private const int W = 0;
    private const int D = 90;
    private const int S = 180;
    private const int A = 270;

    bool canShoot;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        renders = GetComponentsInChildren<Renderer>();
        col = GetComponent<BoxCollider>();

        moveSound = GetComponent<AudioSource>();
        shootSound = muzzle.GetComponent<AudioSource>();

        canShoot = true;

        EventSystem.Singleton.OnNewGame += ProcessNewGame;
        ProcessNewGame();
    }

    private void ProcessNewGame()
    {
        float posX = Settings.Singleton.Size.x / 2;
        float posZ = Settings.Singleton.Size.y / 2;
        transform.position = new Vector3(posX, 0, posZ);
        ModifyRot(W);
        rb.velocity = Vector3.zero;
        foreach (Renderer rend in renders)
        {
            rend.enabled = true;
        }
        col.enabled = true;

        canShoot = true;
        StopAllCoroutines();
    }

    private void Update()
    {
        if (moveSound.isPlaying)
        {
            moveSound.Pause();
        }
        if (Time.timeScale == 0)
        {
            return;
        }
        if (Settings.Singleton.mode == Settings.InputMode.Keyboard)
        {
            ProcessKeyboardModeInput();
        }
        else 
        {
            ProcessMouseKeyboardModeInput();
        }
        ModifyPosIfNeeded();

    }

    void ProcessKeyboardModeInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            ModifyRot(W);
            ApplyForce(W);
        }
        if (Input.GetKey(KeyCode.D))
        {
            ModifyRot(D);
            ApplyForce(D);
        }
        if (Input.GetKey(KeyCode.S))
        {
            ModifyRot(S);
            ApplyForce(S);
        }
        if (Input.GetKey(KeyCode.A))
        {
            ModifyRot(A);
            ApplyForce(A);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }

    }

    void ProcessMouseKeyboardModeInput()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint
            (new Vector3(Input.mousePosition.x, Input.mousePosition.y, 78));
        Vector3 direction = ComputeDirection(mousePos);
        Quaternion targetRot = ComputeTargetRot(direction);
        ModifyRot(targetRot);
        
        if (Input.GetKey(KeyCode.W) 
            || Input.GetMouseButton(1) 
            || Input.GetKey(KeyCode.UpArrow))
        {
            ApplyForce(transform.forward);
        }

        if (Input.GetKeyDown(KeyCode.Space)
            || Input.GetKeyDown(KeyCode.Mouse0))
        {
            Shoot();
        }

    }

    bool ModifyPosIfNeeded()
    {
        Vector2 size = Settings.Singleton.Size;
        if (transform.position.x > size.x)
        {
            transform.position = new Vector3(0, 0, transform.position.z);
        }
        if (transform.position.x < 0)
        {
            transform.position = new Vector3(size.x, 0, transform.position.z);
        }
        if (transform.position.z < 0)
        {
            transform.position = new Vector3(transform.position.x, 0, size.y);
        }
        if (transform.position.z > size.y)
        {
            transform.position = new Vector3(transform.position.x, 0, 0);
        }
        return true;
    }

    Vector3 ComputeDirection(Vector3 mousePos)
    {
        Vector3 shipPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 targetPos = new Vector3(mousePos.x, 0, mousePos.z);
        return targetPos - shipPos;
    }

    Quaternion ComputeTargetRot(Vector3 direction)
    {
        Quaternion targetRot = Quaternion.LookRotation(direction);
        return targetRot;
    }


    void ModifyRot(int targetRot)
    {
        float step = angularSpeed * Time.deltaTime;
        transform.rotation = 
            Quaternion.RotateTowards(
                transform.rotation, 
                Quaternion.Euler(0, targetRot, 0), 
                step); 
    }

    void ModifyRot(Quaternion targetRot)
    {
        float step = angularSpeed * Time.deltaTime;
        transform.rotation =
            Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                step);
    }

    void ApplyForce(int targetRot)
    {
        if (!moveSound.isPlaying)
        { 
            moveSound.Play();
        }
        Vector3 direction = new Vector3(0, 0, 1);
        switch (targetRot)
        {
            case 0:
                direction = new Vector3(0, 0, 1);
                break;
            case 90:
                direction = new Vector3(1, 0, 0);
                break;
            case 180:
                direction = new Vector3(0, 0, -1);
                break;
            case 270:
                direction = new Vector3(-1, 0, 0);
                break;
        }
        forceToApply = direction * acceleration;
    }

    void ApplyForce(Vector3 direction)
    {
        if (!moveSound.isPlaying)
        {
            moveSound.Play();
        }
        forceToApply = direction * acceleration;
    }

    private void FixedUpdate()
    {
        rb.AddForce(forceToApply, ForceMode.Acceleration);
        forceToApply = Vector3.zero;
        if (rb.velocity.sqrMagnitude > maxSpeed * maxSpeed)
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        EventSystem.Singleton.PlayerDestroyed();
        StartCoroutine(Respawn());
    }


    private IEnumerator InvulnerabilityCoroutine()
    {
        col.enabled = false;
        for (int i = 0; i < 6; i++)
        {
            foreach (Renderer rend in renders)
            {
                rend.enabled = false;
            }
            yield return new WaitForSeconds(0.25f);
            foreach (Renderer rend in renders)
            {
                rend.enabled = true;
            }
            yield return new WaitForSeconds(0.25f);
        }
        col.enabled = true;
    }

    private IEnumerator Respawn()
    {
        BecomeInvisible();
        rb.velocity = Vector3.zero;
        float sizeX = Settings.Singleton.Size.x;
        float sizeY = Settings.Singleton.Size.y;

        float rndX = Random.Range(0, sizeX);
        float rndZ = Random.Range(0, sizeY);
        Vector3 rndPos = new Vector3(rndX, 0, rndZ);
        transform.position = rndPos;
        yield return new WaitForSeconds(0.25f);
        StartCoroutine(InvulnerabilityCoroutine());
    }

    private void BecomeInvisible()
    {
        col.enabled = false;
        foreach (Renderer rend in renders)
        {
            rend.enabled = false;
        }
    }


    private void Shoot()
    {
        if (!canShoot)
        {
            return;
        }
        shootSound.Play();
        BulletSpawner.Singleton.Spawn(muzzle.position, muzzle.forward);
        StartCoroutine(ShootWaitTime());
    }

    private IEnumerator ShootWaitTime()
    {
        canShoot = false;
        yield return new WaitForSeconds(0.3f);
        canShoot = true;
    }
}
