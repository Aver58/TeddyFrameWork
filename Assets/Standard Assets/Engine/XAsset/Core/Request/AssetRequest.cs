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
    public Type assetType;

    public LoadedCallback completed;
    public string name;

    public AssetRequest() {
        asset = null;
        LoadState = AssetLoadState.Init;
    }

    public AssetLoadState LoadState { get; protected set; }

    protected void Complete()
    {
        if(completed != null) {
            completed(this);
            completed = null;
        }
    }

    public virtual bool isDone
    {
        get { return LoadState == AssetLoadState.Loaded || LoadState == AssetLoadState.Unload; }
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
#if UNITY_EDITOR
        LoadModule.loadDelegate = UnityEditor.AssetDatabase.LoadAssetAtPath; 
#endif
        if (LoadModule.runtimeMode == false && LoadModule.loadDelegate != null) {
            asset = LoadModule.loadDelegate(name, assetType);
        }

        if (asset == null) {
            error = "error! file not exist:" + name;
            Debug.LogError(error);
            return;
        }
        
        LoadState = AssetLoadState.Loaded;
        Complete();
    }

    internal virtual void Unload()
    {
        if(asset == null)
            return;

        if(!LoadModule.runtimeMode)
            if(!(asset is GameObject))
                Resources.UnloadAsset(asset);

        asset = null;
        LoadState = AssetLoadState.Unload;
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
            GameLog.LogException(ex);
        }

        completed = null;
        return false;
    }

    internal virtual void LoadImmediate(){}

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