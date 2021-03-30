using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArrayExtension
{
    public static T RepeatGet<T>(this T[] array, int index)
    {
        if (index < 0)
        {
            return array.RepeatGet(index + array.Length);
        }
        else
        {
            return array[index % array.Length];
        }
    }

    public static void RepeatSet<T>(this T[] array, int index, T value)
    {
        if (index < 0)
        {
            array.RepeatSet(index + array.Length, value);
        }
        else
        {
            array[index % array.Length] = value;
        }
    }
}
