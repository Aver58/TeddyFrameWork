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
    private AssetLoadState _loadState = AssetLoadState.Init;
    private List<Object> _requires;
    public Type assetType;

    public Action<AssetRequest> completed;
    public string name;

    public AssetRequest()
    {
        asset = null;
        loadState = AssetLoadState.Init;
    }

    public AssetLoadState loadState
    {
        get { return _loadState; }
        protected set
        {
            _loadState = value;
            if(value == AssetLoadState.Loaded)
            {
                Complete();
            }
        }
    }

    private void Complete()
    {
        if(completed != null)
        {
            completed(this);
            completed = null;
        }
    }

    public virtual bool isDone
    {
        get { return loadState == AssetLoadState.Loaded || loadState == AssetLoadState.Unload; }
    }

    public virtual float progress
    {
        get { return 1; }
    }

    public virtual string error { get; protected set; }

    public string text { get; protected set; }

    public byte[] bytes { get; protected set; }

    public Object asset { get; internal set; }

    private bool checkRequires
    {
        get { return _requires != null; }
    }

    private void UpdateRequires()
    {
        for(var i = 0; i < _requires.Count; i++)
        {
            var item = _requires[i];
            if(item != null)
                continue;
            Release();
            _requires.RemoveAt(i);
            i--;
        }

        if(_requires.Count == 0)
            _requires = null;
    }

    internal virtual void Load()
    {
        if(!LoadModule.runtimeMode && LoadModule.loadDelegate != null)
            asset = LoadModule.loadDelegate(name, assetType);
        if(asset == null) error = "error! file not exist:" + name;
        loadState = AssetLoadState.Loaded;
    }

    internal virtual void Unload()
    {
        if(asset == null)
            return;

        if(!LoadModule.runtimeMode)
            if(!(asset is GameObject))
                Resources.UnloadAsset(asset);

        asset = null;
        loadState = AssetLoadState.Unload;
    }

    internal virtual bool Update()
    {
        if(checkRequires)
            UpdateRequires();
        if(!isDone)
            return true;
        if(completed == null)
            return false;
        try
        {
            completed.Invoke(this);
        }
        catch(Exception ex)
        {
            Debug.LogException(ex);
        }

        completed = null;
        return false;
    }

    internal virtual void LoadImmediate()
    {
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