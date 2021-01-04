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
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// 所有界面的抽象类
/// </summary>
public abstract class ViewBase
{
    public ViewID key { get; set; }
    public ViewType viewType { get; set; }
    public float closeTime { get; set; }
    public bool needNavigation { get { return viewType == ViewType.MAIN; } }
    public bool isLoaded { get { return m_loadState == LoadState.LOADED; } }
    public bool isOpen { get; private set; }
    public GameObject gameObject;
    public string panelName;
    public string assetPath;

    /// <summary>
    /// 优化显示，隐藏的时候移到很远
    /// </summary>
    protected bool optimizationVisible = true;
    protected Transform transform;
    protected Dictionary<string, Object> UI;
    /// <summary>
    /// 打开传入的参数
    /// </summary>
    protected UIEventArgs openParam;

    private Transform m_parent;
    private AssetRequest m_assetRequest;
    private Action<ViewBase> m_loadedCallback;
    private LoadState m_loadState;// 加载状态
    protected Vector3 FarAwayPosition = new Vector3(10000, 10000, 0);

    #region API

    public virtual bool CanOpen() { return true; }
    public virtual bool CanClose() { return true; }
    
    /// <summary>
    /// 界面加载
    /// </summary>
    /// <param name="loadedCallback"></param>
    public void Load(Action<ViewBase> loadedCallback = null)
    {
        m_loadedCallback = loadedCallback;
        m_loadState = LoadState.LOADING;
        LoadModule.LoadUI(assetPath, OnLoadCompleted);
    }

    /// <summary>
    /// 界面打开
    /// </summary>
    public void Open()
    {
        isOpen = true;
        SetActive(true);
        AddAllMessage();
        OnOpen(openParam);

        transform.SetAsLastSibling();
    }

    /// <summary>
    /// 界面关闭
    /// </summary>
    public void Close()
    {
        Debug.Log(panelName + "Close");
        closeTime = Time.time;
        openParam = null;

        isOpen = false;
        SetActive(false);
        RemoveAllMessage();
        OnClose();
    }

    /// <summary>
    /// 界面恢复
    /// </summary>
    public void Resume()
    {
        isOpen = true;
        SetActive(true);
    }

    /// <summary>
    /// 界面冻结
    /// </summary>
    public void Freeze()
    {
        isOpen = false;
        SetActive(false);
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

    protected void AddListener(Button button, UnityAction action)
    {
        button.onClick.AddListener(action);
    }

    protected void AddMessage()
    {
        //todo 
    }

    protected void RemoveAllMessage()
    {
        //todo 
    }

    public void SetOpenParam(UIEventArgs args)
    {
        openParam = args;
    }

    #endregion

    #region LifeCycle

    /// <summary>
    /// 按钮监听
    /// </summary>
    protected abstract void AddAllListener();
    /// <summary>
    /// 事件监听
    /// </summary>
    protected abstract void AddAllMessage();
    protected abstract void OnLoaded();
    protected abstract void OnOpen(UIEventArgs args = null);
    protected abstract void OnClose();
    protected virtual void OnUpdate() { }

    #endregion

    #region Private

    private void OnLoadCompleted(AssetRequest request)
    {
        m_assetRequest = request;
        if(!string.IsNullOrEmpty(request.error))
        {
            request.Release();
            Debug.LogError("加载界面失败:" + panelName);
            return;
        }

        m_loadState = LoadState.LOADED;

        // 实例化
        m_parent = UIModule.GetParent(viewType);
        gameObject = GameObject.Instantiate(request.asset as GameObject, m_parent);
        gameObject.SetActive(true);
        gameObject.name = request.asset.name;

        transform = gameObject.transform;
        UI = HierarchyUtil.GetHierarchyItems(gameObject);

        OnLoaded();
        AddAllListener();

        if(m_loadedCallback != null)
        {
            m_loadedCallback(this);
        }
    }

    private void SetActive(bool active)
    {
        if(optimizationVisible)
        {
            Canvas canvas = gameObject.GetComponent<Canvas>();
            if(canvas == null)
            {
                canvas = gameObject.AddComponent<Canvas>();
                gameObject.AddComponent<GraphicRaycaster>();
                canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;
            }

            canvas.overrideSorting = active;
            // 移到很远
            MoveFarAway(!active);
        }
        else
        {
            gameObject.SetActive(active);
        }
    }

    protected void MoveFarAway(bool active)
    {
        transform.localPosition = active ? FarAwayPosition : Vector3.zero;
    }
    #endregion
}