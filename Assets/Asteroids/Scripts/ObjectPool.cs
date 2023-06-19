using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField]
    protected int poolCapacity;
    [SerializeField]
    protected GameObject prefab;

    protected Queue<GameObject> ready;
    protected int amount;

    private void Start()
    {
        amount = 0;
        ready = new Queue<GameObject>();
        for (int i = 0; i < poolCapacity; i++)
        {
            GameObject t = Instantiate(prefab);
            t.SetActive(false);
            ready.Enqueue(t);
            t.name = prefab.name + " " + amount;
            amount++;
        }
        CustomStart();
    }


    protected virtual void CustomStart()
    {
        ;
    }

    public virtual void Spawn(Vector3 pos)
    {
        if (ready.Count == 0)
        {
            ExtendPool();
        }
        GameObject gb = ready.Dequeue();
        gb.transform.position = pos;
        gb.SetActive(true);
    }

    protected void ExtendPool()
    {
        for (int i = 0; i < poolCapacity / 2; i++)
        {
            GameObject t = Instantiate(prefab);
            t.SetActive(false);
            ready.Enqueue(t);
            t.name = prefab.name + " " + amount;
            amount++;
        }
        poolCapacity += poolCapacity / 2;
    }

    public virtual void Despawn(GameObject gb)
    {
        //bullet.transform.position = Vector3.zero;
        gb.SetActive(false);
        ready.Enqueue(gb);
    }
}
