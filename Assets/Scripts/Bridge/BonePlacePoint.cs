using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonePlacePoint : MonoBehaviour
{
    [SerializeField]
    private GameObject bonePrefab;
    [SerializeField]
    private Vector3 rot1;
    [SerializeField]
    private Vector3 rot2;

    [SerializeField]
    private byte amountNeeded;
    private byte amountPlaced;

    private bool isRot1;
    private void Awake()
    {
        isRot1 = true;
        amountPlaced = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        Skeleton skeleton = other.GetComponent<Skeleton>();
        Debug.Log(skeleton + " endter");
        Debug.Log("");
        if (skeleton != null)
        {
            if (skeleton.GetBone())
            {
                Instantiate(bonePrefab, transform.position, Quaternion.Euler(isRot1 ? rot1 : rot2));
                transform.localPosition += new Vector3(0, 0, 0.75f);
                isRot1 = !isRot1;
                amountPlaced++;
                if (amountPlaced >= amountNeeded)
                {
                    GameManager.Singleton.SkeletonFinished(skeleton.GetID());
                    Debug.Log("EndGame");
                }
            }
        }
    }
}
