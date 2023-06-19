using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneSpawnManager : MonoBehaviour
{
    [SerializeField]
    Transform BoneSpawnerParent;

    private List<BoneSpawner> boneSpawners;
    private HashSet<Vector3> readyPositions;

    public static BoneSpawnManager Singleton;
    private void Awake()
    {
        if (Singleton == null)
        { 
         Singleton = this;
        }
        readyPositions = new HashSet<Vector3>();
        boneSpawners = new List<BoneSpawner>();
    }
    private void Start()
    {
        boneSpawners = new List<BoneSpawner>
            (BoneSpawnerParent.GetComponentsInChildren<BoneSpawner>());
        for (byte i = 0; i < boneSpawners.Count; i++)
        {
            readyPositions.Add(boneSpawners[i].transform.position);
            boneSpawners[i].SetId(i);
        }
    }

    internal void OnBoneSpawned(Vector3 pos)
    {
        readyPositions.Add(pos);
    }

    public void BoneTaken(int id)
    {
        readyPositions.Remove(boneSpawners[id].transform.position);
    }

    public Vector3 GetRandomBoneSpawnerPos()
    {
        if (readyPositions.Count >= boneSpawners.Count / 2)
        {
            int timer = 10000;
            int i = 0;
            while ( i < timer)
            {
                Random.InitState(System.DateTime.Now.Millisecond);
                int rnd = Random.Range(0, boneSpawners.Count);
                if (readyPositions.Contains(boneSpawners[rnd].transform.position))
                {
                    readyPositions.Remove(boneSpawners[rnd].transform.position);
                    return boneSpawners[rnd].transform.position;
                }
                i++;
            }
        }
        else
        {
            int timer = 10;
            int i = 0;
            while (i < timer)
            {
                foreach (Vector3 pos in readyPositions)
                {
                    float rnd = Random.value;
                    if (rnd < 1 / readyPositions.Count)
                    {
                        readyPositions.Remove(pos);
                        return pos;
                    }
                }
                i++;
            }
        }
        return Vector3.zero;
        //TODO special case rarity congratulation;
    }
}
