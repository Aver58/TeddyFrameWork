#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    AsyncBundleRequest.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/9 13:13:47
=====================================================
*/
#endregion

using UnityEngine;

public class BundleRequestAsync : BundleRequest
{
    private AssetBundleCreateRequest _request;

    public override float progress
    {
        get
        {
            if(isDone) 
                return 1;
            if(loadState == AssetLoadState.Init) 
                return 0;
            if(_request == null) 
                return 1;
            return _request.progress;
        }
    }

    internal override bool Update()
    {
        if(!base.Update()) 
            return false;

        if(loadState == AssetLoadState.LoadAsset)
            if(_request.isDone)
            {
                assetBundle = _request.assetBundle;
                if(assetBundle == null) 
                    error = string.Format("unable to load assetBundle:{0}", name);
                loadState = AssetLoadState.Loaded;
                return false;
            }

        return true;
    }

    internal override void Load()
    {
        if(_request == null)
        {
            _request = AssetBundle.LoadFromFileAsync(name);
            if(_request == null)
            {
                error = name + " LoadFromFile failed.";
                return;
            }

            loadState = AssetLoadState.LoadAsset;
        }
    }

    internal override void Unload()
    {
        _request = null;
        loadState = AssetLoadState.Unload;
        base.Unload();
    }

    internal override void LoadImmediate()
    {
        Load();
        assetBundle = _request.assetBundle;
        if(assetBundle != null) 
            GameLog.LogWarning("LoadImmediate:" + assetBundle.name);
        loadState = AssetLoadState.Loaded;
    }
}