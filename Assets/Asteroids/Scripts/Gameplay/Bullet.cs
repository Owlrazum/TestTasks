using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Shiftable
{
    [SerializeField]
    private float travelDistance;

    private float passedDistance;

    private const int UFOLAYER = 6;
    private const int PLAYERLAYER = 7;

    public void Init(Vector3 directionArg, int layer)
    {
        direction = directionArg;
        passedDistance = travelDistance;
        if (layer == UFOLAYER)
        {
            GetComponent<Renderer>().material.color = Color.red;
        }
        else if (layer == PLAYERLAYER)
        {
            GetComponent<Renderer>().material.color = new Color(0.5f, 0.7f, 0.3f);
        }
    }

    protected override void ProcessNewGame()
    {
        Deactivate();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Deactivate();
    }

    protected override void Shift() 
    {
        Vector3 translation = speedModifier * new Vector3(direction.x, 0, direction.z) * Time.deltaTime;
        transform.Translate(translation);
        passedDistance -= translation.magnitude;
        if (passedDistance < 0)
        {
            Deactivate();
        }
    }

    private void Deactivate()
    {
        BulletSpawner.Singleton.Despawn(gameObject);
    }
}
