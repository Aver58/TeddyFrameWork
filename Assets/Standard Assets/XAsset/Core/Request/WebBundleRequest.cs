#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    WebBundleRequest.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/9 21:37:11
=====================================================
*/
#endregion

using UnityEngine;
using UnityEngine.Networking;

public class WebBundleRequest : BundleRequest
{
    private UnityWebRequest _request;
    public bool cache;
    public Hash128 hash;

    public override float progress
    {
        get
        {
            if(isDone) return 1;
            if(loadState == LoadState.Init) return 0;

            if(_request == null) return 1;

            return _request.downloadProgress;
        }
    }

    internal override void Load()
    {
        //_request = cache ? UnityWebRequestAssetBundle.GetAssetBundle(name, hash)
        //                 : UnityWebRequestAssetBundle.GetAssetBundle(name);
        //_request.SendWebRequest();
        //loadState = LoadState.LoadAssetBundle;
    }

    internal override void Unload()
    {
        if(_request != null)
        {
            _request.Dispose();
            _request = null;
        }

        loadState = LoadState.Unload;
        base.Unload();
    }
}