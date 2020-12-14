#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    WebAssetRequest.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/9 21:38:56
=====================================================
*/
#endregion

using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;
public class WebAssetRequest : AssetRequest
{
    private UnityWebRequest _www;

    public override float progress
    {
        get
        {
            if(isDone) return 1;
            if(loadState == AssetLoadState.Init) return 0;

            if(_www == null) return 1;

            return _www.downloadProgress;
        }
    }

    public override string error
    {
        get { return _www.error; }
    }

    internal override bool Update()
    {
        if(!base.Update()) 
            return false;

        if(loadState == AssetLoadState.LoadAsset)
        {
            if(_www == null)
            {
                error = "www == null";
                return false;
            }

            if(!string.IsNullOrEmpty(_www.error))
            {
                error = _www.error;
                loadState = AssetLoadState.Loaded;
                return false;
            }

            if(_www.isDone)
            {
                GetAsset();
                loadState = AssetLoadState.Loaded;
                return false;
            }

            return true;
        }

        return true;
    }

    private void GetAsset()
    {
        if(assetType == typeof(Texture2D))
            asset = DownloadHandlerTexture.GetContent(_www);
        else if(assetType == typeof(AudioClip))
            asset = DownloadHandlerAudioClip.GetContent(_www);
        else if(assetType == typeof(TextAsset))
            text = _www.downloadHandler.text;
        else
            bytes = _www.downloadHandler.data;
    }

    internal override void Load()
    {
        if(assetType == typeof(AudioClip))
        {
            _www = UnityWebRequestMultimedia.GetAudioClip(name, AudioType.WAV);
        }
        else if(assetType == typeof(Texture2D))
        {
            _www = UnityWebRequestTexture.GetTexture(name);
        }
        else
        {
            _www = new UnityWebRequest(name);
            _www.downloadHandler = new DownloadHandlerBuffer();
        }

        _www.SendWebRequest();
        loadState = AssetLoadState.LoadAsset;
    }

    internal override void Unload()
    {
        if(asset != null)
        {
            Object.Destroy(asset);
            asset = null;
        }

        if(_www != null)
            _www.Dispose();

        bytes = null;
        text = null;
        loadState = AssetLoadState.Unload;
    }
}