using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Singleton;
    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
    }

    public event Action<byte> OnSkeletonFinished;
    public void SkeletonFinished(byte id)
    {
        OnSkeletonFinished?.Invoke(id);
    }

}
