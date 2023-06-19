using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class UFO : Shiftable
{
    [SerializeField]
    private Player player;

    private const int UFOLAYER = 6; 

    private Renderer rend;
    private Rigidbody rb;
    private BoxCollider coll;
    private AudioSource aud;

    IEnumerator shootCoroutine;
    IEnumerator spawnCoroutine;

    private float moveSpeed;
    private bool isSpawning;
    private bool isShooting;
    protected override void CustomStart()
    {
        rend = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<BoxCollider>();
        aud = GetComponent<AudioSource>();
        moveSpeed = speedModifier;
        shootCoroutine = ShootCoroutine();
        isSpawning = false;
        isShooting = false;
        Reset();
    }
    protected override void ProcessNewGame()
    {
        Reset();
    }
    private void Reset()
    {
        SetSpawningCoroutine(false);
        rb.isKinematic = true;
        rend.enabled = false;
        coll.enabled = false;
        speedModifier = 0;
        SetShootingCoroutine(false);
        SetShiftingCoroutine(false);
        SetSpawningCoroutine(true, Random.Range(20, 40));
        //StartCoroutine(SpawnCoroutine(Random.Range(20, 40)));
    }

    private void SetSpawningCoroutine(bool shouldRun, float waitTime = 0)
    {
        if (shouldRun && !isSpawning)
        {
            spawnCoroutine = SpawnCoroutine(waitTime);
            StartCoroutine(spawnCoroutine);
            isSpawning = true;
        }
        else if (!shouldRun && isSpawning)
        {
            StopCoroutine(spawnCoroutine);
            isSpawning = false;
        }
    }
    private IEnumerator SpawnCoroutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (Random.value < 0.5f)
        {
            float posX = 0;
            float posZ = Random.Range(18, 72);
            transform.position = new Vector3(posX, 0, posZ);
            direction = new Vector3(1, 0, 0);
            speedModifier = moveSpeed;
        }
        else 
        {
            float posX = Settings.Singleton.Size.x;
            float posZ = Random.Range(18, 72);
            transform.position = new Vector3(posX, 0, posZ);
            direction = new Vector3(-1, 0, 0);
            speedModifier = moveSpeed;
        }
        rend.enabled = true;
        coll.enabled = true;
        rb.isKinematic = false;
        SetShootingCoroutine(true);
        SetShiftingCoroutine(true);
        isSpawning = false;
    }

    private void SetShootingCoroutine(bool shouldShoot)
    {
        if (shouldShoot && !isShooting)
        {
            isShooting = true;
            StartCoroutine(shootCoroutine);
        }
        else if (!shouldShoot && isShooting)
        {
            isShooting = false;
            StopCoroutine(shootCoroutine);
        }
    }

    private IEnumerator ShootCoroutine()
    {
        while (isShooting)
        { 
            Vector3 direction = player.transform.position - transform.position;
            BulletSpawner.Singleton.Spawn(transform.position, direction.normalized, UFOLAYER);
            aud.Play();
            float reloadTime = Random.Range(2, 5);
            yield return new WaitForSeconds(reloadTime);
        }
    }

    protected override void ModifyPosIfNeeded()
    {
        if (Settings.Singleton == null)
        {
            Debug.Log(gameObject.name);
        }
        Vector2 size = Settings.Singleton.Size;

        if (transform.position.x > size.x
            || transform.position.x < 0
            || transform.position.z < 0
            || transform.position.z > size.y)
        {
            Reset();
        }
    }



    private void OnCollisionEnter(Collision collision)
    {
        Bullet bullet = collision.collider.GetComponent<Bullet>();
        bool isByPlayer = false;
        if (bullet != null)
        {
            if (bullet.gameObject.layer == 7)
            {
                isByPlayer = true;
            }
        }
        if (collision.collider.GetComponent<Player>())
        {
            isByPlayer = true;
        }
        EventSystem.Singleton.UFODestroyed(isByPlayer);
        Reset();
    }
}
