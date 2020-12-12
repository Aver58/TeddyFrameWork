#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    BundleAssetRequest.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/9 13:18:16
=====================================================
*/
#endregion

using System.Collections.Generic;
using System.IO;

public class BundleAssetRequest : AssetRequest
{
    protected readonly string assetBundleName;
    protected BundleRequest BundleRequest;
    protected List<BundleRequest> children = new List<BundleRequest>();

    public BundleAssetRequest(string bundle)
    {
        assetBundleName = bundle;
    }

    internal override void Load()
    {
        BundleRequest = LoadModule.LoadBundle(assetBundleName);
        var names = LoadModule.GetAllDependencies(assetBundleName);
        foreach(var item in names) 
            children.Add(LoadModule.LoadBundle(item));
        var assetName = Path.GetFileName(name);
        var ab = BundleRequest.assetBundle;
        if(ab != null) 
            asset = ab.LoadAsset(assetName, assetType);
        if(asset == null) 
            error = "asset == null";
        loadState = AssetLoadState.Loaded;
    }

    internal override void Unload()
    {
        if(BundleRequest != null)
        {
            BundleRequest.Release();
            BundleRequest = null;
        }

        if(children.Count > 0)
        {
            foreach(var item in children) 
                item.Release();
            children.Clear();
        }

        asset = null;
    }
}