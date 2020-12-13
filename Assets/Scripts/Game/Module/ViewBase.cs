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
    public float closeTime { get; set; }
    public bool needNavigation { get { return viewType == ViewType.MAIN; } }
    public bool IsLoaded { get { return m_loadState == ViewLoadState.LOADED; } }
    public bool IsOpen { get { return m_loadState == ViewLoadState.LOADED; } }
    public GameObject gameObject;

    private Transform parent;
    protected Transform transform;
    protected Dictionary<string, Object> UI;
    private Action<AssetRequest> m_loadedCallback;
    private AssetRequest m_assetRequest;
    private string m_panelName;
    private bool m_isOpen;
    private UIEventArgs m_openParam;// TODO: 打开的参数
    private ViewLoadState m_loadState;// 加载状态

    
    public ViewBase()
    {
        
    }

    #region API

    public void Load(Action<AssetRequest> loadedCallback = null)
    {
        m_loadedCallback = loadedCallback;
        m_loadState = ViewLoadState.LOADING;
        LoadModule.LoadAsset(m_panelName, typeof(GameObject), OnLoadCompleted);
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

    public void Close()
    {
        closeTime = Time.time;
    }

    public void Dispose()
    {
        if(m_assetRequest != null)
            LoadModule.UnloadAsset(m_assetRequest);

        if(UI != null && UI.Count > 0)
            UI.Clear();

        if(gameObject != null)
            GameObject.Destroy(gameObject);
    }

    public virtual void OnOpen() { }
    public virtual void OnLoaded() { }
    public virtual void OnClose() { }
    public virtual void OnUpdate() { }

    public void SetPanelName(string name)
    {
        m_panelName = name;
    }

    public void SetOpenParam(UIEventArgs args)
    {
        m_openParam = args;
    }

    #endregion

    #region Private


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
        m_assetRequest = request;
        if(!string.IsNullOrEmpty(request.error))
        {
            request.Release();
            Debug.LogError("加载界面失败:" + m_panelName);
            return;
        }

        // 实例化
        parent = UIModule.GetParent(viewType);
        gameObject = GameObject.Instantiate(transform.gameObject, parent);
        gameObject.SetActive(true);
        gameObject.name = request.asset.name;

        m_loadState = ViewLoadState.LOADED;
        transform = gameObject.transform;
        UI = HierarchyUtil.GetHierarchyItems(gameObject);

        OnLoaded();
        RealOpen();

        if(m_loadedCallback != null)
        {
            m_loadedCallback(request);
        }
    }

    #endregion
}