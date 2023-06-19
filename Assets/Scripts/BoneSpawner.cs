using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject bonePrefab;

    private byte id;

    public void SetId(byte idArg)
    {
        id = idArg;
    }


    private void Start()
    {
        SpawnBone();
    }
    public void ProcessBonePickUp()
    {
        BoneSpawnManager.Singleton.BoneTaken(id);
        StartCoroutine(SpawnCoroutine());
    }
    
    private IEnumerator SpawnCoroutine()
    {
        yield return new WaitForSeconds(6);
        SpawnBone();
    }

    private void SpawnBone()
    {
        GameObject gb = Instantiate(bonePrefab, transform);
        gb.GetComponent<BonePickUp>().SetBoneSpawner(this); // should never fail (prefab should contain valid BonePickUp)
        BoneSpawnManager.Singleton.OnBoneSpawned(transform.position);
    }
}
