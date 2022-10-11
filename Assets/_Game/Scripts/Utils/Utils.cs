using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

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

    /// <summary>
    /// returns index of insertion
    /// </summary>
    public static int AddSorted<T>(this List<T> list, T value)
    {
        int index = list.BinarySearch(value);
        list.Insert((index >= 0) ? index : ~index, value);
        return index;
    }
}