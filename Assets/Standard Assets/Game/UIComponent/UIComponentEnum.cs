using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public enum UIComponentEnum
{
    ETCJoystick,
    Button,
    Image,
    ImageEx,
    Text,
    TextMeshProUGUI,
    RectTransform,
}

public static class UIComponentType
{
    public static readonly Type[] TypeArray = null;
    public const int MAX_NUM = (int)UIComponentEnum.RectTransform + 1;

    static UIComponentType()
    {
        TypeArray = new Type[MAX_NUM];
        TypeArray[(int)UIComponentEnum.ETCJoystick] = typeof(ETCJoystick);
        TypeArray[(int)UIComponentEnum.Button] = typeof(Button);
        TypeArray[(int)UIComponentEnum.Image] = typeof(Image);
        TypeArray[(int)UIComponentEnum.ImageEx] = typeof(ImageEx);
        TypeArray[(int)UIComponentEnum.Text] = typeof(Text);
        TypeArray[(int)UIComponentEnum.TextMeshProUGUI] = typeof(TextMeshProUGUI);
        TypeArray[(int)UIComponentEnum.RectTransform] = typeof(RectTransform);
    }
}