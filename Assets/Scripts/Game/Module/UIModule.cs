#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    UIModule.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/12 11:33:03
=====================================================
*/
#endregion

/*
框架具体实现的功能和需求
    加载，显示，隐藏，关闭页面，根据标示获得相应界面实例
    界面层级管理
    界面导航
    界面通用对话框管理(多类型Message Box)
    便于进行需求和功能扩展(比如，在跳出页面之前添加逻辑处理等)
 */

using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// UI框架
/// </summary>
public class UIModule : ModuleBase
{
    private static ViewBase curView;
    private static List<ViewBase> views;
    private static Dictionary<ViewID, ViewBase> viewMap;

    public UIModule()
    {
        // 随缘10个
        viewMap = new Dictionary<ViewID, ViewBase>(10);
    }

    #region API

    public static void OpenView(ViewID key, EventArgs eventArgs)
    {
        if(OpenCheck(key))
            return;

        var view = GetView(key);
        if(view != null)
        {

            // 已加载过
            if(view.IsLoaded)
            {
                InitView(view);
            }
            else
            {
                view.Load(delegate{ InitView(view); });    
            }
        }
        else
        {
            Debug.LogError("界面实例化失败"+ key.ToString());
        }
    }

    public static void CloseView()
    {

    }

    #endregion

    #region Private
    // 界面打开前的检测
    private static bool OpenCheck(ViewID key)
    {
        return true;
    }

    private static ViewBase GetView(ViewID key)
    {
        ViewBase view;
        if(viewMap.TryGetValue(key,out view))
            return view;

        ViewDefine.ViewConfig viewConfig;
        ViewDefine.ViewMapping.TryGetValue(key, out viewConfig);
        Type viewClass = viewConfig.viewClass;

        // 反射拿到实例
        view = Activator.CreateInstance(viewClass) as ViewBase;
        if(view != null)
            return view;

        return null;
    }

    //初始化界面管理器状态
    private static void InitView(ViewBase view)
    {
        viewMap.Add(view.Key, view);
        bool needNav = AddToNavigationStack(view);
        if(needNav)
            OnNavigationStack();
        
        view.Open();
    }

    // 加入导航栈
    private static bool AddToNavigationStack(ViewBase view)
    {
        if(view.needNavigation)
        {
            views.Add(view);
            return true;
        }
        return false;
    }

    //导航栈主逻辑
    private static void OnNavigationStack()
    {
        curView = views[views.Count - 1];
        // 上一个界面隐藏

        // 当前界面显示
    }

    #endregion
}