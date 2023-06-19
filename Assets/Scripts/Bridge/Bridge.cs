                                                      using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    [SerializeField]
    private byte ID;

    private void Start()
    {
        ColorManager.Singleton.OnAssignColor += OnColorAssigned;
    }

    private void OnColorAssigned(byte playerID, Color color)
    {
        if (playerID != ID)
        {
            return;
        }
        UpdateColors(color);
        ColorManager.Singleton.OnAssignColor -= OnColorAssigned;
    }

    private void UpdateColors(Color color)
    {
        var renders = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renders)
        {
            r.material.color = color;
        }
    }
}
