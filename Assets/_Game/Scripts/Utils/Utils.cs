using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        for (int i = n - 1; i >= 1; i--)
        {
            int j = Random.Range(0, i + 1);

            T value = list[j];
            list[j] = list[i];
            list[i] = value;
        }
    }
}