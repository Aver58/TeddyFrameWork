#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    ExportPanelHierarchy.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/14 15:05:29
=====================================================
*/
#endregion

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ExportPanelHierarchy
{
    private static System.Type[] ms_componentTypes =
    {
        typeof(Button),
        typeof(Image),
        typeof(Text),
    };

    public static void ExportNested(Object obj)
    {
        GameObject root = obj as GameObject;
        if(root == null) return;

        UIHierarchy hierarchy = root.GetComponent<UIHierarchy>();
        if(hierarchy == null)
        {
            hierarchy = root.AddComponent<UIHierarchy>();
        }

        //生成根节点层级
        List<UIHierarchy.EffectItemInfo> fxFields = new List<UIHierarchy.EffectItemInfo>();
        List<UIHierarchy.ItemInfo> fields = new List<UIHierarchy.ItemInfo>();
        GetChildComponentUtilHierarchy(root.transform, fxFields, fields);
        hierarchy.SetEffects(fxFields);
        hierarchy.SetWidgets(fields);

        //生成子panel层级
        UIHierarchy[] childHierarchys = root.GetComponentsInChildren<UIHierarchy>(true);
        for(int i = 1; i < childHierarchys.Length; i++)
        {
            UIHierarchy childHrcy = childHierarchys[i];

            List<UIHierarchy.EffectItemInfo> childFx = new List<UIHierarchy.EffectItemInfo>();
            List<UIHierarchy.ItemInfo> childUIItem = new List<UIHierarchy.ItemInfo>();
            GetChildComponentUtilHierarchy(childHrcy.transform, childFx, childUIItem);
            childHrcy.SetEffects(childFx);
            childHrcy.SetWidgets(childUIItem);
        }


        EditorUtility.SetDirty(root);
        AssetDatabase.SaveAssets();
    }

    //导出传入节点的层级，直到某个子节点挂有UIHierarchy组件
    private static void GetChildComponentUtilHierarchy(Transform transRoot, List<UIHierarchy.EffectItemInfo> fxFields, List<UIHierarchy.ItemInfo> fields)
    {
        for(int i = 0; i < transRoot.childCount; i++)
        {
            Transform trans = transRoot.GetChild(i);

            UIHierarchy hrchy = trans.GetComponent<UIHierarchy>();
            if(hrchy != null)
            {
                fields.Add(new UIHierarchy.ItemInfo(hrchy.name, hrchy));
                continue;
            }

            //FxExportItem fxItem = trans.GetComponent<FxExportItem>();
            //if(fxItem != null)
            //{
            //    Object fieldItem = GetChildComponent(fxItem.gameObject) ?? fxItem.transform;
            //    fxFields.Add(new UIHierarchy.EffectItemInfo(fxItem.name, fieldItem, fxItem.transform.parent));
            //}
            //else
            //{
                UIExportItem uiItem = trans.GetComponent<UIExportItem>();
                if(uiItem != null)
                {
                    Object fieldItem = GetChildComponent(uiItem.gameObject);
                    if(fieldItem == null)
                    {
                        fieldItem = uiItem.transform;
                    }
                    fields.Add(new UIHierarchy.ItemInfo(uiItem.name, fieldItem));
                }
            //}
            GetChildComponentUtilHierarchy(trans, fxFields, fields);
        }
    }

    static Object GetChildComponent(GameObject go)
    {
        Object component = null;
        for(int i = 0; i < ms_componentTypes.Length; ++i)
        {
            component = go.GetComponent(ms_componentTypes[i]);
            if(component != null)
                break;
        }
        return component;
    }
}