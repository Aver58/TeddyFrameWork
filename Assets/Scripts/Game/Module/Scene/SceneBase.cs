#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    SceneBase.cs
 Author:      Zeng Zhiwei
 Time:        2020\12\18 星期五 22:16:42
=====================================================
*/
#endregion

using System;

public abstract class SceneBase
{
    public string panelName;
    public string assetPath;
    public SceneID SceneID { get; set; }
    public LoadState LoadState { get; set; }
    private Action<AssetRequest> m_loadedCallback;
    private AssetRequest m_assetRequest;

    public bool IsLoading()
    {
        return LoadState == LoadState.LOADING;
    }

    public void Dispose()
    {
        if(m_assetRequest != null)
            LoadModule.UnloadAsset(m_assetRequest);
    }

    public void Load(Action<AssetRequest> loadedCallback = null)
    {
        m_loadedCallback = loadedCallback;
        LoadState = LoadState.LOADING;
        LoadModule.LoadSceneAsync(assetPath, false,OnLoadCompleted);
    }

    private void OnLoadCompleted(AssetRequest assetRequest)
    {
        m_assetRequest = assetRequest;
        if(!string.IsNullOrEmpty(assetRequest.error))
        {
            assetRequest.Release();
            GameLog.LogError("加载场景失败:" + panelName);
            return;
        }

        LoadState = LoadState.LOADED;

        if(m_loadedCallback != null)
            m_loadedCallback.Invoke(assetRequest);
    }

    #region LifeCycle
    protected abstract void OnLoaded();
    public abstract void OnEnter();
    public abstract void OnExit();
    public abstract void OnUpdate();
    #endregion
}