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
    完整的UI生命周期管理【加载，显示，隐藏，关闭页面，根据标示获得相应界面实例】
    完整的数据更新方案【MVC】
    UI组，
    界面层级管理
    界面导航
    https://zhuanlan.zhihu.com/p/102278660
 */

using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// UI框架
/// </summary>
public class UIModule : ModuleBase
{
    private static UIModule instance;
    public static UIModule Instance {
        get {
            if(instance == null) {
                instance = new UIModule();
            }

            return instance;
        }
    }

    // 上次检查缓存的时间
    private float m_lastCacheCheckTime = 0f;
    // 已打开过的view哈希表
    private Dictionary<ViewID, ViewBase> m_viewMap;
    private static Dictionary<ViewType, Transform> m_viewRoot;
    private const float UI_PANEL_CACHE_TIME = 5;
    private static GameObject m_UIMask;
    private const string m_prefabPath = "common/UIMask";
    
    private readonly Vector3 HIDE_POINT = new Vector3(0, -10000, 0);

    private GameObject gameObjectPoolRoot;
    public GameObject GameObjectPoolRoot {
        get {
            if (gameObjectPoolRoot == null) {
                gameObjectPoolRoot = new GameObject("GameObjectPool");
                gameObjectPoolRoot.transform.position = HIDE_POINT;
            }

            return gameObjectPoolRoot;
        }
    }

    private UINavigation uiNavigation;

    public UIModule() {
        uiNavigation = new UINavigation();
        uiNavigation.Init();
        m_viewMap = new Dictionary<ViewID, ViewBase>();
        m_viewRoot = new Dictionary<ViewType, Transform> {
            {ViewType.MAIN,GameObject.Find("Canvas/Main").transform },
            {ViewType.POPUP,GameObject.Find("Canvas/Popup").transform },
            {ViewType.FIXED,GameObject.Find("Canvas/Fixed").transform },
        };
    }

    #region API

    // public void OpenView(ViewID key, object args0 = null) { }
    // public void OpenView(ViewID key, object args0 = null, object args1 = null) { }
    public void OpenView(ViewID key, object[] args = null)
    {
        var view = GetView(key);

        if(view != null) {
            view.SetOpenParam(args);
            if (BeforeOpen(view) == false) {
                return;
            }

            // 已加载过
            if(view.IsLoaded) {
                InitView(view);
            } else {
                view.Load(InitView);    
            }
        } else {
            GameLog.LogError("[UIModule]界面实例化失败" + key.ToString());
        }
    }

    public void CloseView(ViewID key)
    {
        ViewBase view = GetView(key);
        if(view != null)
        {
            if(view.IsOpen)
            {
                view.Close();
                uiNavigation.RemoveLastItem(view);
            }
            else
            {
                GameLog.LogWarning("[UIModule]界面关闭失败，已经关闭！" + key.ToString());
            }
        }
        else
        {
            GameLog.LogError("[UIModule]界面关闭失败，没有找到指定界面！" + key.ToString());
        }
    }

    public Transform GetParentTransform(ViewType viewType) {
        return m_viewRoot[viewType];
    }

    // View通过这个接口拿父节点
    public static Transform GetParent(ViewType viewType)
    {
        return m_viewRoot[viewType];
    }

    // PopupViewBase 拿Mask，全局就一个
    public static GameObject GetUIMask()
    {
        if(m_UIMask == null)
        {
            AssetRequest assetRequest = LoadModule.LoadUI(m_prefabPath);
            var asset = assetRequest.asset as GameObject;
            m_UIMask = GameObject.Instantiate(asset, GetParent(ViewType.POPUP));
            m_UIMask.transform.SetAsFirstSibling();
            m_UIMask.transform.localPosition = Vector3.zero;
            return m_UIMask;
        }
        return m_UIMask;
    }

    #region Scene

    public static void SceneEnter()
    {
    
    }

    /// <summary>
    /// 切换场景，资源管理相关
    /// </summary>
    public void SceneExit() {
        foreach(var item in m_viewMap)
        {
            var view = item.Value;
            if(view.IsOpen)
                view.Close();
        }
    }

    #endregion

    #endregion

    #region Private
    public override void Update(float dt)
    {
        // 已打开界面的update,有需求才做
        ViewBase curView = uiNavigation.GetLastItem();
        if(curView != null && curView.IsOpen)
            curView.Update();

        // 自动回收cache的界面，todo这个是不是可以用个协程做
        var curTime = Time.time;
        if(curTime - m_lastCacheCheckTime > 1)
        {
            foreach(var item in m_viewMap)
            {
                var view = item.Value;
                //Debug.Log((view.closeTime - curTime > UIPANEL_CACHE_TIME).ToString());
                if(view.closeTime - curTime > UI_PANEL_CACHE_TIME && !view.IsOpen)
                {
                    m_viewMap[view.ViewID] = null;
                    view.Dispose();
                }
            }
        }
    }

    // 界面打开前的检测
    private static bool BeforeOpen(ViewBase view)
    {
        // 做一些界面打开前的检查操作
        return view.CanOpen();
    }

    // 界面关闭前的检测
    private static bool BeforeClose(ViewBase view)
    {
        return view.CanClose();
    }

    private ViewBase GetView(ViewID key)
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
        {
            view.ViewID = key;
            view.assetPath = viewConfig.path;
            view.panelName = viewConfig.name;
            return view;
        }

        return null;
    }

    //初始化界面管理器状态
    private void InitView(ViewBase view)
    {
        m_viewMap[view.ViewID] = view;

        view.Open();
        view.Active();
        view.Refresh();
        uiNavigation.Push(view);
        //--抛出界面打开完成事件
        //--设置场景摄像机状态
    }

    #endregion
}