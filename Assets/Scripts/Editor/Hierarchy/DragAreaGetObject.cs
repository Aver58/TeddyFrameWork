#region Copyright © 2018 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    DragAreaGetObject.cs
 Author:      Zeng Zhiwei
 Time:        2019/11/19 14:45:41
=====================================================
*/
#endregion

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class DragAreaGetObject : Editor
{
    public static List<Object> GetDropObjs(string msg = null)
    {
        Event current = Event.current;

        GUI.contentColor = Color.white;

        var dragArea = GUILayoutUtility.GetRect(0f, 25f, GUILayout.ExpandWidth(true));

        GUIContent title = new GUIContent(msg);
        if (string.IsNullOrEmpty(msg))
        {
            title = new GUIContent("Drag Object here from Project view to get the object");
        }

        GUI.Box(dragArea, title);
        List<Object> tempList = new List<Object>();

        switch (current.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dragArea.Contains(current.mousePosition))
                    break;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                if (current.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    for (int i = 0; i < DragAndDrop.objectReferences.Length; ++i)
                    {
                        var temp = DragAndDrop.objectReferences[i];

                        if (temp == null)
                            break;
                        tempList.Add(temp);
                    }

                    return tempList;
                }

                Event.current.Use();
                break;
            default:
                break;
        }
        return tempList;
    }

    public static UnityEngine.Object GetDropObj(string msg = null)
    {
        var list = GetDropObjs();
        if (list !=null)
        {
            return GetDropObjs()[0];
        }
        return null;
    }
}