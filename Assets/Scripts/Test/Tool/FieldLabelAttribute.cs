using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

/// <summary>
/// 能让字段在inspector面板显示中文字符
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class FieldLabelAttribute : PropertyAttribute {
    public string label;//要显示的字符
    public FieldLabelAttribute(string label) {
        this.label = label;
    }
}

#if UNITY_EDITOR
//绑定特性描述类
[CustomPropertyDrawer(typeof(FieldLabelAttribute))]
public class FieldLabelDrawer : PropertyDrawer {
    private FieldLabelAttribute FieldLabelAttribute {
        get { return (FieldLabelAttribute)attribute; }
    }
    /// <summary>
    /// 重写OnGui方法
    /// </summary>
    /// <param name="position"></param>
    /// <param name="property"></param>
    /// <param name="label"></param>
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label) {
        //在这里重新绘制
        var str = ((FieldLabelAttribute)attribute).label;
        // UnityEngine.Debug.LogError("str : " + str);
        // UnityEngine.Debug.LogError("label : " + label);
        EditorGUI.PropertyField(rect, property, new GUIContent(str, label.text), true);
    }
}
#endif