using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.Debug;

public static class Extensions
{
    public static void SetRectFrom(this RectTransform rectTransform, RectTransform other)
    {
        rectTransform.pivot = other.pivot;
        rectTransform.anchorMin = other.anchorMin;
        rectTransform.anchorMax = other.anchorMax;
        rectTransform.anchoredPosition = other.anchoredPosition;
        rectTransform.sizeDelta = other.sizeDelta;
    }

    public static void Stretch(this RectTransform rectTransform)
    {
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;
    }

    public static Transform[] GetChildren(this Transform transform)
    {
        var children = new Transform[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
            children[i] = transform.GetChild(i);

        return children;
    }

    public static int Remap(this int value, int from1, int to1, int from2, int to2) => (int)Remap((float)value, from1, to1, from2, to2);

    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}