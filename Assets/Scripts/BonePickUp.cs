using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BonePickUp : MonoBehaviour
{
    private BoxCollider collider;
    private void Start()
    {
        collider = GetComponent<BoxCollider>();
    }

    public void SetBoneSpawner(BoneSpawner sp)
    {
        spawnPoint = sp;
    }

    private BoneSpawner spawnPoint;
    private void OnTriggerEnter(Collider other)
    {
        Skeleton skeleton = other.GetComponent<Skeleton>();
        if (skeleton != null)
        {
            skeleton.PickBone(transform);
            OnBonePickedUp();
        }
    }

    private void OnBonePickedUp()
    {
        spawnPoint.ProcessBonePickUp();
        collider.enabled = false;
    }

}
