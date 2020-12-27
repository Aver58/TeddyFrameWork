using UnityEngine;
using System.Collections;

public static class ComponentExtensions
{
    public static RectTransform rectTransform(this Component cp)
    {
        return cp.transform as RectTransform;
    }

    public static void SetWidth(this RectTransform rt, float width)
    {
        rt.sizeDelta = new Vector2(width, rt.sizeDelta.y);
    }

    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
