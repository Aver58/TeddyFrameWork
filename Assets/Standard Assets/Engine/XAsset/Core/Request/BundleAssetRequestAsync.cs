#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    BundleAssetRequestAsync.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/9 21:22:04
=====================================================
*/
#endregion

using System.IO;
using UnityEngine;

public class BundleAssetRequestAsync : BundleAssetRequest
{
    private AssetBundleRequest _request;

    public BundleAssetRequestAsync(string bundle)
        : base(bundle)
    {
    }

    public override float progress
    {
        get
        {
            if(isDone) return 1;

            if(LoadState == AssetLoadState.Init) return 0;

            if(_request != null) return _request.progress * 0.7f + 0.3f;

            if(BundleRequest == null) return 1;

            var value = BundleRequest.progress;
            var max = children.Count;
            if(max <= 0)
                return value * 0.3f;

            for(var i = 0; i < max; i++)
            {
                var item = children[i];
                value += item.progress;
            }

            return value / (max + 1) * 0.3f;
        }
    }

    private bool OnError(BundleRequest bundleRequest)
    {
        error = bundleRequest.error;
        if(!string.IsNullOrEmpty(error))
        {
            LoadState = AssetLoadState.Loaded;
            return true;
        }

        return false;
    }

    internal override bool Update()
    {
        if(!base.Update()) return false;

        if(LoadState == AssetLoadState.Init) return true;

        if(_request == null)
        {
            if(!BundleRequest.isDone) return true;
            if(OnError(BundleRequest)) return false;

            for(var i = 0; i < children.Count; i++)
            {
                var item = children[i];
                if(!item.isDone) return true;
                if(OnError(item)) return false;
            }

            var assetName = Path.GetFileName(name);
            _request = BundleRequest.assetBundle.LoadAssetAsync(assetName, assetType);
            if(_request == null)
            {
                error = "request == null";
                LoadState = AssetLoadState.Loaded;
                return false;
            }

            return true;
        }

        if(_request.isDone)
        {
            asset = _request.asset;
            LoadState = AssetLoadState.Loaded;
            if(asset == null) error = "asset == null";
            return false;
        }

        return true;
    }

    internal override void Load()
    {
        BundleRequest = LoadModule.LoadBundleAsync(assetBundleName);
        var bundles = LoadModule.GetAllDependencies(assetBundleName);
        foreach(var item in bundles) children.Add(LoadModule.LoadBundleAsync(item));
        LoadState = AssetLoadState.LoadAssetBundle;
    }

    internal override void Unload()
    {
        _request = null;
        LoadState = AssetLoadState.Unload;
        base.Unload();
    }

    internal override void LoadImmediate()
    {
        BundleRequest.LoadImmediate();
        foreach(var item in children) item.LoadImmediate();
        if(BundleRequest.assetBundle != null)
        {
            var assetName = Path.GetFileName(name);
            asset = BundleRequest.assetBundle.LoadAsset(assetName, assetType);
        }

        LoadState = AssetLoadState.Loaded;
        if(asset == null) error = "asset == null";
    }
}