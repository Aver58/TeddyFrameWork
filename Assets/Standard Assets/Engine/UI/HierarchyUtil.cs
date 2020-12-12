#region Copyright © 2018 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    HierarchyUtil.cs
 Author:      Zeng Zhiwei
 Time:        2019/11/20 14:20:58
=====================================================
*/
#endregion

using System.Collections.Generic;
using UnityEngine;

public static class HierarchyUtil
{
    public static Dictionary<string, Object> GetHierarchyItems(GameObject go)
    {
        if (go == null)
        {
            Debug.LogError("【HierarchyUtil.ExportToTarget】go is null！！！");
            return null;
        }

        UIHierarchy uiHierarchy = go.GetComponent<UIHierarchy>();
        if (uiHierarchy == null)
        {
            Debug.LogError(go.name + "没有UIHierarchy组件！！");
            return null;
        }

        Dictionary<string, Object> m_ui = new Dictionary<string, Object>();
        foreach (UIHierarchy.ItemInfo itemInfo in uiHierarchy.widgets)
        {
            m_ui[itemInfo.name] = itemInfo.item;
        }

        foreach (UIHierarchy.EffectItemInfo itemInfo in uiHierarchy.effects)
        {
            // TODO:
            //m_ui[itemInfo.name] = itemInfo.item;
        }

        foreach (UIHierarchy.ItemInfo itemInfo in uiHierarchy.externals)
        {
            m_ui[itemInfo.name] = itemInfo.item;
        }
        return m_ui;
    }

    public static void Test()
    {
        Debug.Log("Test");
    }
}

