#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    AssetRequest.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/9 12:28:32
=====================================================
*/
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class AssetRequest : Reference, IEnumerator
{
    private List<Object> _requires;
    private LoadState _loadState = LoadState.Init;

    public string name;
    public Type assetType;
    public Action<AssetRequest> OnCompleted;
    public virtual bool isDone { get { return loadState == LoadState.Loaded || loadState == LoadState.Unload; } }
    public virtual float progress { get { return 1; } }
    public Object asset { get; internal set; }
    public virtual string error { get; protected set; }
    public string text { get; protected set; }
    public byte[] bytes { get; protected set; }
    public LoadState loadState
    {
        get { return _loadState; }
        protected set
        {
            _loadState = value;
            if(value == LoadState.Loaded)
                Complete();
        }
    }

    public AssetRequest()
    {
        asset = null;
        loadState = LoadState.Init;
    }

    internal virtual void Load()
    {
        if(Assets.runtimeMode == false && Assets.loadDelegate != null)
            asset = Assets.loadDelegate(name, assetType);

        if(asset == null)
            error = "error! file not exist:" + name;

        loadState = LoadState.Loaded;
    }

    internal virtual void Unload()
    {
        if(asset == null)
            return;

        if(Assets.runtimeMode == false)
            if((asset is GameObject) == false)
                Resources.UnloadAsset(asset);

        asset = null;
        loadState = LoadState.Unload;
    }

    internal virtual bool Update()
    {
        if(_requires != null)
            UpdateRequires();
        if(!isDone)
            return true;
        if(OnCompleted == null)
            return false;

        try
        {
            OnCompleted.Invoke(this);
        }
        catch(Exception ex)
        {
            Debug.LogException(ex);
        }

        OnCompleted = null;
        return false;
    }

    private void UpdateRequires()
    {
        for(int i = 0; i < _requires.Count; i++)
        {
            Object item = _requires[i];
            if(item != null)
                continue;
            Release();
            _requires.RemoveAt(i);
            i--;
        }

        if(_requires.Count == 0)
            _requires = null;
    }

    internal virtual void LoadImmediate() { }

    private void Complete()
    {
        if(OnCompleted != null)
        {
            OnCompleted(this);
            OnCompleted = null;
        }
    }

    #region IEnumerator implementation

    public bool MoveNext()
    {
        return !isDone;
    }

    public void Reset()
    {
    }

    public object Current
    {
        get { return null; }
    }

    #endregion
}