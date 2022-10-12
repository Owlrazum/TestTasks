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
        int insertionIndex = (index >= 0) ? index : ~index;
        list.Insert(insertionIndex, value);
        return insertionIndex;
    }

    public static int AddSorted<T>(this List<T> list, T value, IComparer<T> comparer)
    { 
        int index = list.BinarySearch(value, comparer);
        int insertionIndex = (index >= 0) ? index : ~index;
        list.Insert(insertionIndex, value);
        return insertionIndex;
    }
}