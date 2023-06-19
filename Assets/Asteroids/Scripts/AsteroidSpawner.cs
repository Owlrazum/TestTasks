using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AsteroidSpawner : ObjectPool
{
    public static AsteroidSpawner Singleton;
    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
    }
    [SerializeField]
    private AudioClip bigExplSound;
    [SerializeField]
    private AudioClip mediumExplSound;
    [SerializeField]
    private AudioClip smallExplSound;

    private AudioSource aud;
    private int round = 2;

    protected override void CustomStart()
    {
        aud = GetComponent<AudioSource>();
        RandomSpawn(2);
        EventSystem.Singleton.OnNewGame += Reset;
    }

    private void Reset()
    {
        round =  1;
    }

    private void RandomSpawn(int amount)
    {
        Debug.Log("RandomSpawn");
        //Random.InitState((int)System.DateTime.Now.Ticks);
        float sizeX = Settings.Singleton.Size.x;
        float sizeY = Settings.Singleton.Size.y;
        for (int i = 0; i < amount; i++)
        {
            float rndX = Random.Range(0, sizeX);
            float rndZ = Random.Range(0, sizeY);
            if (Random.value < 0.5f) // X
            {
                rndZ = sizeY - rndX > rndX ? sizeY : 0;
            }
            else // Z
            {
                rndX = sizeX - rndX > rndX ? sizeX : 0;
            }
            Vector3 rndPos = new Vector3(rndX, 0, rndZ);

            rndX = Random.Range(-1.0f, 1.0f);
            rndZ = Random.Range(-1.0f, 1.0f);
            Vector3 rndDir = new Vector3(rndX, 0, rndZ);
            if (rndDir.normalized == Vector3.zero)
            {
                rndDir *= 100;
            }
            rndDir.Normalize();
            Spawn(rndPos, rndDir, Asteroid.Size.Big);
        }
    }

    public void Spawn(Vector3 pos, Vector3 direction, Asteroid.Size size)
    {
        if (ready.Count == 0)
        {
            ExtendPool();
        }

        GameObject ast = ready.Dequeue();
        ast.transform.position = pos;
        ast.GetComponent<Asteroid>().Init(direction, size);
        ast.SetActive(true);
    }

    public void DoubleSpawn(Vector3 pos, Vector3 direction, Asteroid.Size size)
    {
        if (ready.Count <= 1)
        {
            ExtendPool();
        }
        Vector3 dir1 = Quaternion.AngleAxis(45, transform.up) * direction;
        Vector3 dir2 = Quaternion.AngleAxis(-45, transform.up) * direction;

        GameObject ast = ready.Dequeue();
        ast.transform.position = pos;
        float speed = ast.GetComponent<Asteroid>().Init(dir1, size);
        ast.SetActive(true);

        ast = ready.Dequeue();
        ast.transform.position = pos;
        ast.GetComponent<Asteroid>().Init(dir2, size, speed);
        ast.SetActive(true);
    }

    public void Despawn(GameObject gb, Asteroid.Size size)
    {
        switch (size)
        {
            case Asteroid.Size.Big:
                aud.PlayOneShot(bigExplSound, 0.7f);
                break;
            case Asteroid.Size.Medium:
                aud.PlayOneShot(mediumExplSound, 0.7f);
                break;
            case Asteroid.Size.Small:
                aud.PlayOneShot(smallExplSound, 0.7f);
                break;
        }
        Despawn(gb);
    }

    public override void Despawn(GameObject gb)
    {
        gb.SetActive(false);
        ready.Enqueue(gb);
        StartCoroutine(CheckExistence());
    }



    private IEnumerator CheckExistence()
    {
        if (ready.Count != poolCapacity)
        {
            yield break;
        }
        yield return new WaitForSeconds(1);
/*        if (ready.Count != poolCapacity)
        {
            yield break;
        }*/
        Debug.Log("======== all ready " + ready.Count + " " + poolCapacity);
        round++;
        Debug.Log(round);
        RandomSpawn(round);
    }
}
