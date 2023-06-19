using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(MoveMobile))]
[RequireComponent(typeof(NavMeshAgent))]
public class AIBot : MonoBehaviour
{
    private enum Type
    {
        Fast,
        Medium,
        Slow
    }

    [SerializeField]
    private Type type;

    [SerializeField]
    private Skeleton skelet;

    [SerializeField]
    private Transform bridgeEntry;

    [SerializeField]
    private Transform bridgePlacePoint;

    private MoveMobile mov;
    private NavMeshAgent agent;
    private bool isPickingUp;
    private bool isStanding;
    private bool isFinished;
    private byte boneCountGoal;
    private void Awake()
    {
        mov = GetComponent<MoveMobile>();
        agent = GetComponent<NavMeshAgent>();
        isStanding = false;
    }
    private void Start()
    {
        GameManager.Singleton.OnSkeletonFinished += CalmDown;
        switch (type)
        {
            case Type.Fast:
                boneCountGoal = 2;
                break;
            case Type.Medium:
                boneCountGoal = 4;
                break;
            case Type.Slow:
                boneCountGoal = 6;
                break;
        }
        StartCoroutine(CollectBonesCoroutine());
    }

    private void CalmDown(byte id)
    {
        StopAllCoroutines();
    }


    public void SetPickingUp(bool isPickingUpArg)
    {
        isPickingUp = isPickingUpArg;
    }
    private IEnumerator CollectBonesCoroutine()
    {
        while (true)
        {
            isStanding = false;
            Vector3 targetPos = BoneSpawnManager.Singleton.GetRandomBoneSpawnerPos();
            //Debug.Log(targetPos);
            while (!isPickingUp)
            {
                Vector3 dir = targetPos - transform.position;
                if (!isPickingUp && dir.sqrMagnitude < agent.radius && !isStanding)
                {
                    Debug.Log("Stand");
                    mov.UpdateMoveControl(0, Vector3.zero);
                    isStanding = true;
                }
                if (dir.sqrMagnitude > agent.radius)
                {
                    mov.UpdateMoveControl(1, dir.normalized);
                    isStanding = false;
                }
                if (!isStanding)
                { 
                    mov.UpdateMoveControl(1, dir.normalized);
                }
                yield return null;
            }
            mov.UpdateMoveControl(0, Vector3.zero);
            yield return new WaitForSeconds(3);

            if (skelet.GetBoneCount() >= boneCountGoal)
            {
                StartCoroutine(PlaceBonesCoroutine());
                yield break;
            }
        }
    }

    private IEnumerator PlaceBonesCoroutine()
    {
        Vector3 targetPos = bridgeEntry.transform.position;
        Vector3 dir = targetPos - transform.position;
        while (dir.sqrMagnitude > 2)
        {
            dir = targetPos - transform.position;
            mov.UpdateMoveControl(1, dir.normalized);
//            Debug.Log(dir.sqrMagnitude);
            yield return null;
        }
        while (skelet.GetBoneCount() > 0)
        {
            targetPos = bridgePlacePoint.transform.position; // place point moves
            dir = targetPos - transform.position;
            Debug.Log("2   ===   " + dir.sqrMagnitude);
            mov.UpdateMoveControl(1, dir.normalized);

            yield return null;
        }
        StartCoroutine(CollectBonesCoroutine());
        yield break;
    }

}
