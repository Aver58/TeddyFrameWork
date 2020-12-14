#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    BundleRequest.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/9 12:30:11
=====================================================
*/
#endregion

using UnityEngine;

public class BundleRequest : AssetRequest
{
    public string assetBundleName { get; set; }
    public AssetBundle assetBundle
    {
        get { return asset as AssetBundle; }
        internal set { asset = value; }
    }

    internal override void Load()
    {
        asset = AssetBundle.LoadFromFile(name);
        if(assetBundle == null)
            error = name + " LoadFromFile failed.";
        loadState = AssetLoadState.Loaded;
    }

    internal override void Unload()
    {
        if(assetBundle == null)
            return;
        assetBundle.Unload(true);
        assetBundle = null;
        loadState = AssetLoadState.Unload;
    }
}