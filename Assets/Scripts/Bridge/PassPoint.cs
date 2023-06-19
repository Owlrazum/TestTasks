using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PassPoint : MonoBehaviour
{
    [SerializeField]
    private byte PassID;
    [SerializeField]
    private Renderer rend;
    [SerializeField]
    private NavMeshObstacle obs;

    private void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Skeleton player = other.GetComponent<Skeleton>();
        if (player != null)
        {
            if (player.GetID() == PassID)
            {
                obs.enabled = false;
                rend.enabled = false;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Skeleton player = other.GetComponent<Skeleton>();
        if (player != null)
        {
            if (player.GetID() == PassID)
            {
                obs.enabled = true;
                rend.enabled = true;
            }
        }
    }
}
