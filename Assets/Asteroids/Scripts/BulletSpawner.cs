using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : ObjectPool
{
    public static BulletSpawner Singleton;
    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
    }

    public void Spawn(Vector3 pos, Vector3 direction, int layer = 7)
    {
        if (ready.Count == 0)
        {
            ExtendPool();
        }
        GameObject bullet = ready.Dequeue();
        bullet.layer = layer;
        bullet.transform.position = pos;
        bullet.GetComponent<Bullet>().Init(direction, layer);
        bullet.SetActive(true);
    }
}
