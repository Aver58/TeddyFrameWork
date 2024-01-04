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
using System;
using Object = UnityEngine.Object;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// DoNothing | HideOther | NeedBack<para/>
/// DoNothing：不关闭其他界面<para/>
/// HideOther：显示时，关闭其他界面。关闭时，将其他页面处理为同时关闭，就达到了禁止返回上一页面的目的（例如：主面板-》子面板-》点击子面板确定按钮-》不需要返回主面板，主子面板全关）<para/>
/// NeedBack：显示时，要前台显示；关闭时，显示前台后面的页面（只要其他页面不处理，就自动退回前一页面了）<para/>
/// 注意：NeedBack或HideOther的页面只能关闭或返回同类型页面
/// </summary>
public enum UIMode
{
    DoNothing,      // 不关闭其他界面
    HideOther,      // 关闭其他界面
    NeedBack,       // 点击返回按钮关闭当前,不关闭其他界面(需要调整好层级关系)
    //NoNeedBack,		// 关闭TopBar,关闭其他界面,不加入backSequence队列
}

/// <summary>
/// 所有界面的抽象类
/// </summary>
public abstract class ViewBase {
    public ViewID ViewID { get; set; }
    public ViewType ViewType { get; set; }
    public UIMode UiMode;
    public float closeTime { get; set; }
    public bool NeedNavigation { get { return UiMode == UIMode.HideOther || UiMode == UIMode.NeedBack; } }
    public bool IsLoaded { get { return m_loadState == LoadState.LOADED; } }
    public bool IsOpen { get; private set; }
    private GameObject gameObject;
    protected Transform transform;
    public string panelName;
    public string assetPath;

    // 优化显示，隐藏的时候移到很远
    private bool optimizationVisible = true;

    // 打开传入的参数
    private object[] openParam;

    private Transform m_parent;
    private AssetRequest m_assetRequest;
    private Action<ViewBase> m_loadedCallback;// 加载回调
    private LoadState m_loadState;// 加载状态
    protected Vector3 FarAwayPosition = new Vector3(10000, 10000, 0);

    protected ViewBase() {}
    protected ViewBase(GameObject go,Transform parent) {
        if(go == null) {
            GameLog.LogError("[ViewBase]构造初始化，没有传入GameObject！");
            return;
        }
        gameObject = go;
        transform = go.transform;

        BindView();
    }

    #region API

    public bool CanOpen() { return true; }
    public bool CanClose() { return true; }

    // 界面加载
    public void Load(Action<ViewBase> loadedCallback = null)
    {
        m_loadedCallback = loadedCallback;
        m_loadState = LoadState.LOADING;
        LoadModule.LoadUI(assetPath, OnLoadCompleted);
    }

    // 界面打开
    public void Open()
    {
        IsOpen = true;
        SetActive(true);
        AddAllMessage();
        OnOpen(openParam);

        transform.SetAsLastSibling();
    }

    // 界面关闭
    public void Close()
    {
        GameLog.Log(panelName + "Close");
        closeTime = Time.time;
        openParam = null;

        IsOpen = false;
        SetActive(false);
        RemoveAllMessage();
        OnClose();
    }

    // 界面刷新
    public void Refresh() {
        OnRefresh();
    }

    // 界面显示
    public void Active() {
        IsOpen = true;
        SetActive(true);
        OnActive();
    }

    // 界面隐藏
    public void Hide() {
        IsOpen = false;
        SetActive(false);
        OnHide();
    }

    public void Dispose()
    {
        if(m_assetRequest != null)
            LoadModule.UnloadAsset(m_assetRequest);

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

    public void SetOpenParam(object[] args)
    {
        openParam = args;
    }

    public void Update()
    {
        OnUpdate();
    }

    public ViewBase GenerateOne(Type itemClass, GameObject prefab, Transform parent)
    {
        Transform transParent = parent == null ? null : parent.transform;
        GameObject go = Object.Instantiate(prefab, transParent, false);
        go.name = prefab.name;

        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
        go.transform.SetAsLastSibling();
        go.SetActive(true);

        var itemInstance = Activator.CreateInstance(itemClass,new object[] { go , transParent });
        ViewBase view = itemInstance as ViewBase;

        return view;
    }

    #endregion

    #region LifeCycle

    protected virtual void AddAllListener() { }// 按钮监听
    protected virtual void AddAllMessage() { }// 事件监听
    protected virtual void OnLoaded() { }
    protected virtual void OnOpen(object[] args) { }
    protected virtual void OnRefresh(){}
    protected virtual void OnClose() { }
    protected virtual void OnActive() { }
    protected virtual void OnHide() { }
    protected virtual void OnUpdate() { }
    protected virtual void BindView() { }

    #endregion

    #region Private

    private void OnLoadCompleted(AssetRequest request) {
        m_assetRequest = request;
        if(!string.IsNullOrEmpty(request.error)) {
            request.Release();
            GameLog.LogError("加载界面失败:" + panelName);
            return;
        }

        m_loadState = LoadState.LOADED;

        // 实例化
        m_parent = UIModule.GetParent(ViewType);
        gameObject = GameObject.Instantiate(request.asset as GameObject, m_parent);
        gameObject.SetActive(true);
        gameObject.name = request.asset.name;

        transform = gameObject.transform;
        BindView();

        OnLoaded();
        AddAllListener();

        if(m_loadedCallback != null) {
            m_loadedCallback(this);
        }
    }

    // popup view, 锚定在哪个节点下
    private void AnchorUIGameObject() {

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