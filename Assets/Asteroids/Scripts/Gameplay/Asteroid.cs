using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Asteroid : Shiftable
{
    public enum Size
    {
        Big = 3,
        Medium = 2, 
        Small = 1,
        None = 0
    }
    private Size size;
    private Size smallerSize;

    private Rigidbody rb;

    [SerializeField]
    private GameObject BigAst;

    [SerializeField]
    private GameObject MediumAst;

    [SerializeField]
    private GameObject SmallAst;


    public float Init(Vector3 directionArg, Size sizeInit, float speed = -1)
    {
        rb = GetComponent<Rigidbody>();
        direction = directionArg;
        size = sizeInit;
        if (size == Size.Big)
        {
            smallerSize = Size.Medium;
            transform.localScale = BigAst.transform.localScale;
        }
        else if (size == Size.Medium)
        {
            smallerSize = Size.Small;
            transform.localScale = MediumAst.transform.localScale;
        }
        else if (size == Size.Small)
        {
            smallerSize = Size.None;
            transform.localScale = SmallAst.transform.localScale;
        }
        else
        {
            Debug.LogError("There is no valid size for this asteroid! (enum.none)");
        }
        if (speed == -1)
        {
            speedModifier = Random.Range
            (
                speedModifier - speedModifier / 3,
                speedModifier + speedModifier / 3
            );
        }
        else
        {
            speedModifier = speed;
        }
        rb.isKinematic = false;
        return speedModifier;
    }

    protected override void ProcessNewGame()
    {
        if (gameObject.activeSelf)
        { 
            AsteroidSpawner.Singleton.Despawn(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        rb.isKinematic = true;
        if (size > Size.Small)
        {
            if (collision.collider.GetComponent<Player>() == null
                && collision.collider.GetComponent<UFO>() == null)
            { 
                AsteroidSpawner.Singleton.DoubleSpawn(transform.position, direction, smallerSize);
            }
        }
        Bullet bullet = collision.collider.GetComponent<Bullet>();
        if (bullet != null )
        {
            if (bullet.gameObject.layer == 7)
            { 
                EventSystem.Singleton.AsteroidDestroyed(size);
            }
        }
        AsteroidSpawner.Singleton.Despawn(gameObject, size);
    }
}
