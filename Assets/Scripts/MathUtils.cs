using System.Collections.Generic;
using UnityEngine;

public static class MathUtils
{
    static public T GetRandomElement<T>(this List<T> list)
    {
        if (list == null || list.Count == 0)
        {
            return default(T);
        }
        int randomIndex = Random.Range(0, list.Count);
        return list[randomIndex];
    }
}
