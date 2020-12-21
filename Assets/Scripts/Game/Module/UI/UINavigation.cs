#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    UINavigation.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/15 15:50:50
=====================================================
*/
#endregion

using System.Collections.Generic;
using UnityEngine;

public class UINavigation
{
    private static int count;
    public static List<ViewBase> History;

    public static void Init()
    {
        if(History != null) 
            return;
        History = new List<ViewBase>();
    }

    public static void Clear()
    {
        History.Clear();
    }

    public static void AddItem(ViewBase view)
    {
        if(view == null)
            return;

        if(view.needNavigation)
        {
            // 弹出窗体后面的窗体冻结
            ViewBase lastView = GetLastItem();
            if(lastView != null)
                lastView.Freeze();
            //Debug.Log("AddItem：" + view.ToString());
            History.Add(view);
        }
    }

    public static void RemoveLastItem(ViewBase view)
    {
        if(view == null)
            return;
        
        if(view.needNavigation)
        {
            // 从栈中移除
            ViewBase lastView = GetLastItem();
            if(lastView == view)
            {
                count = History.Count;
                if(count == 0)
                    return;
                //Debug.Log("RemoveLastItem：" + view.ToString());
                History.RemoveAt(count - 1);
            }
        }
    }

    public static ViewBase GetLastItem()
    {
        count = History.Count;
        if(count == 0)
            return null;
        ViewBase data = History[count - 1];

        return data;
    }
}