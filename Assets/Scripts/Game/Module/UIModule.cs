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
    unity ui框架层级管理
    https://www.jianshu.com/p/666de6c7695a?utm_campaign=maleskine&utm_content=note&utm_medium=seo_notes&utm_source=recommendations
 */

using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// UI框架
/// </summary>
public class UIModule : ModuleBase
{
    /// <summary>
    /// 上次检查缓存的时间
    /// </summary>
    private float m_lastCacheCheckTime = 0f;
    /// <summary>
    /// 导航栈
    /// </summary>
    private static Stack<ViewBase> m_viewStack;
    /// <summary>
    /// 已打开过的view哈希表
    /// </summary>
    private static Dictionary<ViewID, ViewBase> m_viewMap;
    private static Dictionary<ViewType, Transform> m_viewRoot;
    private static float UIPANEL_CACHE_TIME = ViewDefine.UIPANEL_CACHE_TIME;

    public UIModule()
    {
        // 随缘10个
        m_viewMap = new Dictionary<ViewID, ViewBase>(10);
        m_viewRoot = new Dictionary<ViewType, Transform>
        {
            {ViewType.MAIN,GameObject.Find("Canvas/Main").transform },
            {ViewType.POPUP,GameObject.Find("Canvas/Popup").transform },
            {ViewType.FIXED,GameObject.Find("Canvas/Fixed").transform },
        };
    }

    #region API

    public static Transform GetParent(ViewType viewType)
    {
        return m_viewRoot[viewType];
    }

    public static void OpenView(ViewID key, EventArgs eventArgs)
    {
        if(BeforeOpen(key))
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

    public static void CloseView(ViewID key)
    {
        ViewBase view = GetView(key);
        bool needNav = PopStack(view);

    }

    #region Scene

    public static void SceneEnter()
    {

    }

    public static void SceneExit()
    {

    }

    #endregion

    #endregion

    #region Private
    private void Update()
    {
        // 已打开界面的update

        // 自动回收cache的界面，todo这个是不是可以用个协程做
        var curTime = Time.time;
        if(curTime - m_lastCacheCheckTime > 1)
        {
            foreach(var item in m_viewMap)
            {
                var view = item.Value;
                if(view.closeTime - curTime > UIPANEL_CACHE_TIME && view.IsOpen)
                {
                    m_viewMap[view.Key] = null;
                    view.Dispose();
                }
            }
        }
    }

    // 界面打开前的检测
    private static bool BeforeOpen(ViewID key)
    {
        // 做一些界面打开前的检查操作
        return true;
    }

    private static ViewBase GetView(ViewID key)
    {
        ViewBase view;
        if(m_viewMap.TryGetValue(key,out view))
            return view;

        ViewConfig viewConfig;
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
        m_viewMap.Add(view.Key, view);
        PushStack(view);
        
        view.Open();
    }

    // 加入导航栈
    private static bool PushStack(ViewBase view)
    {
        // 上一个界面隐藏
        // 当前界面显示
        if(view.needNavigation)
        {
            m_viewStack.Push(view);
            return true;
        }
        return false;
    }

    // 返回导航栈上一个界面
    private static bool PopStack(ViewBase view)
    {
        // 当前界面隐藏，从栈中移除
        // 上一个界面显示
        ViewBase stackTopView = m_viewStack.Peek();
        if(stackTopView == view)
        {
            m_viewStack.Pop();
            return true;
        }
        return false;
    }

    //导航栈主逻辑
    private static void OnNavigationStack()
    {
        //curView = m_viewStack.Peek();
    
    }

    #endregion
}