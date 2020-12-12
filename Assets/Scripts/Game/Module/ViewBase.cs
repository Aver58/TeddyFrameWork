#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    ViewBase.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/12 11:34:11
=====================================================
*/
#endregion

using UnityEngine;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;

public class ViewBase
{

    public ViewID Key { get; set; }
    public ViewType viewType { get; set; }
    public bool needNavigation { get { return viewType == ViewType.MAIN; } }

    private Transform parent;
    protected Transform transform;
    protected Dictionary<string, Object> UI;
    private Action<AssetRequest> m_loadedCallback;

    private string m_panelName;
    private bool m_isOpen;
    //private object m_openParam;// TODO: 打开的参数
    private ViewLoadState m_loadState;// 加载状态

    public bool IsLoaded { get { return m_loadState == ViewLoadState.LOADED; } }
    
    public ViewBase()
    {
        parent = GameObject.Find("Canvas").transform;
    }

    public void SetPanelName(string name)
    {
        m_panelName = name;
    }

    public void Load(Action<AssetRequest> loadedCallback = null)
    {
        m_loadedCallback = loadedCallback;
        m_loadState = ViewLoadState.LOADING;
        LoadModule.LoadAsset(m_panelName,typeof(GameObject), OnLoadCompleted);
    }

    public void Open()
    {
        if(ViewLoadState.LOADED == m_loadState)
        {

        }
        else
        {
            Load();
        }
    }

    private void RealOpen()
    {
        if(!m_isOpen)
        {
            OnOpen();
            m_isOpen = true;
        }
    }

    private void OnLoadCompleted(AssetRequest request)
    {
        if(!string.IsNullOrEmpty(request.error))
        {
            request.Release();
            Debug.LogError("加载界面失败:" + m_panelName);
            return;
        }

        // instance
        GameObject go = GameObject.Instantiate(transform.gameObject, parent);
        go.SetActive(true);
        go.name = request.asset.name;

        m_loadState = ViewLoadState.LOADED;
        transform = go.transform;
        UI = HierarchyUtil.GetHierarchyItems(go);

        if(m_loadedCallback != null)
        {
            m_loadedCallback(request);
        }

        OnLoaded();
        RealOpen();
    }

    public virtual void OnOpen() { }
    public virtual void OnLoaded() { }
    public virtual void OnClose() { }
    public virtual void OnUpdate() { }
}