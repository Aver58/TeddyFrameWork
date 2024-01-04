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

// 导航栈，（树状会不会更好管理？）
// 控制每个页面关闭后返回前一页面（1>2>3>4>5）
public class UINavigation {
    private int count;
    private List<ViewBase> openedViews;

    public void Init() {
        if (openedViews == null) {
            openedViews = new List<ViewBase>();
        }
    }

    public void Clear() {
        openedViews.Clear();
    }

    public void Push(ViewBase view) {
        if (view == null) {
            return;
        }

        if(view.NeedNavigation) {
            //Debug.Log("AddItem：" + view.ToString());
            // 如果UI页面在UI栈中任何一个位置，将其调出（从原位置移除，再放到栈顶——List的最后）
            var isFound = false;
            for (int i = 0; i < openedViews.Count; i++) {
                if (openedViews[i].ViewID == view.ViewID) {
                    openedViews.RemoveAt(i);
                    openedViews.Add(view);
                    isFound = true;
                    break;
                }
            }

            if (!isFound) {
                openedViews.Add(view);
            }

            view.Active();

            if (view.UiMode == UIMode.HideOther) {
                for (int i = 1; i < openedViews.Count; i++) {
                    if (openedViews[i].IsOpen) {
                        openedViews[i].Hide();
                    }
                }
            }
        }
    }

    public void RemoveLastItem(ViewBase view) {
        if(view == null)
            return;
        
        if(view.NeedNavigation)
        {
            // 从栈中移除
            ViewBase lastView = GetLastItem();
            if(lastView == view)
            {
                count = openedViews.Count;
                if(count == 0)
                    return;
                //Debug.Log("RemoveLastItem：" + view.ToString());
                openedViews.RemoveAt(count - 1);
            }
        }
    }

    public ViewBase GetLastItem() {
        count = openedViews.Count;
        if(count == 0)
            return null;
        ViewBase data = openedViews[count - 1];

        return data;
    }
}