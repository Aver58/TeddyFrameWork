#region Copyright © 2018 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    UIHierarchyEditor.cs
 Author:      Zeng Zhiwei
 Time:        2019/11/19 14:39:41
=====================================================
*/
#endregion


using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(UIHierarchy))]
public class UIHierarchyEditor:Editor
{
    private SerializedProperty externalsProperty;
    private List<Object> tempList = null;

    void OnEnable()
    {
        externalsProperty = serializedObject.FindProperty("externals");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();
        GUILayout.Space(10);

        tempList = DragAreaGetObject.GetDropObjs("Drop Objects Here To add to UIHierarchy Externals Propertys");

        if (tempList != null)
        {
            foreach (Object item in tempList)
            {
                externalsProperty.InsertArrayElementAtIndex(externalsProperty.arraySize);
                var itemInfo = externalsProperty.GetArrayElementAtIndex(externalsProperty.arraySize - 1);
                itemInfo.FindPropertyRelative("name").stringValue = item.name;
                itemInfo.FindPropertyRelative("item").objectReferenceValue = item;
            }
        }

        GUILayout.Space(10);
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
        //ShowGameObjectList(externalsProperty);

        serializedObject.ApplyModifiedProperties();
    }

    void ShowGameObjectList(SerializedProperty list)
    {
        for (int i = 0; i < list.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();

            var itemInfo = list.GetArrayElementAtIndex(i);
            var keyProperty = itemInfo.FindPropertyRelative("name");
            var gogoProperty = itemInfo.FindPropertyRelative("item");

            keyProperty.stringValue = EditorGUILayout.TextField(keyProperty.stringValue);
            gogoProperty.objectReferenceValue = EditorGUILayout.ObjectField(gogoProperty.objectReferenceValue, typeof(GameObject), true) as GameObject;

            if (GUILayout.Button("-", GUILayout.Height(16), GUILayout.Width(20)))
            {
                list.DeleteArrayElementAtIndex(i);
                i -= 1;
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}

