using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public static ColorManager Singleton;
    private void Awake()
    {
        if (Singleton == null)
        { 
            Singleton = this;
        }
    }

    [SerializeField]
    private Color p1;
    [SerializeField]
    private Color p2;
    [SerializeField]
    private Color p3;

    private void Start() // execution order is after default time;
    {
        //StartCoroutine(AssignWhenReady());
        AssignColors();
    }

/*    private IEnumerator AssignWhenReady()
    {
        while (true)
        {
            if (OnAssignColor.GetInvocationList().Length > 0)
            { 
                AssignColors();
                yield break;
            }
            yield return null;
        }
    }*/

    public event Action<byte, Color> OnAssignColor;
    private void AssignColors()
    {
        OnAssignColor?.Invoke(0, p1);
        OnAssignColor?.Invoke(1, p2);
        OnAssignColor?.Invoke(2, p3);
    }
}
