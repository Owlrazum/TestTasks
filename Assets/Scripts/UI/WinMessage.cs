using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinMessage : MonoBehaviour
{
    void Start()
    {
        GameManager.Singleton.OnSkeletonFinished += ProcessEnd;
    }

    private void ProcessEnd(byte id)
    {
        if (id == 0)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(true);
        }
    }
}
