using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Singleton;
    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
    }
    [SerializeField]
    private int amountToPool;
    [SerializeField]
    private GameObject prefab;

    private Queue<GameObject> pooledBones;
    private void Start()
    {
        pooledBones = new Queue<GameObject>();
        for (int i = 0; i < amountToPool; i++)
        {
            GameObject t = Instantiate(prefab);
            t.SetActive(false);
            pooledBones.Enqueue(t);
        }
    }

    public GameObject GetPooled()
    {
        return pooledBones.Dequeue();
    }

    public void SetPooled(GameObject t)
    {
        t.SetActive(false);
        pooledBones.Enqueue(t);
    }
}
