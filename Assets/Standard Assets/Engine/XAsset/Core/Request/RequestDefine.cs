#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    Request.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/9 10:21:53
=====================================================
*/
#endregion

/// <summary>
/// 加载状态
/// </summary>
public enum AssetLoadState
{
    Init,
    LoadAssetBundle,
    LoadAsset,
    Loaded,
    Unload
}