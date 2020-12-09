#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    ManifestRequest.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/9 14:24:34
=====================================================
*/
#endregion

using System.IO;

/// <summary>
/// Manifest加载请求
/// </summary>
public class ManifestRequest : AssetRequest
{
    private string assetName;
    private BundleRequest request;

    public int version { get; private set; }
    public override float progress
    {
        get
        {
            if(isDone) 
                return 1;

            if(loadState == LoadState.Init) 
                return 0;

            if(request == null) 
                return 1;

            return request.progress;
        }
    }

    internal override void Load()
    {
        assetName = Path.GetFileName(name);
        if(Assets.runtimeMode)
        {
            var assetBundleName = assetName.Replace(".asset", ".unity3d").ToLower();
            request = Assets.LoadBundleAsync(assetBundleName);
            loadState = LoadState.LoadAssetBundle;
        }
        else
        {
            loadState = LoadState.Loaded;
        }
    }

    internal override bool Update()
    {
        if(!base.Update()) 
            return false;

        if(loadState == LoadState.Init) 
            return true;

        if(request == null)
        {
            loadState = LoadState.Loaded;
            error = "request == null";
            return false;
        }

        if(request.isDone)
        {
            if(request.assetBundle == null)
            {
                error = "assetBundle == null";
            }
            else
            {
                var manifest = request.assetBundle.LoadAsset<Manifest>(assetName);
                if(manifest == null)
                    error = "manifest == null";
                else
                    Assets.OnManifestLoaded(manifest);
            }

            loadState = LoadState.Loaded;
            return false;
        }

        return true;
    }
}