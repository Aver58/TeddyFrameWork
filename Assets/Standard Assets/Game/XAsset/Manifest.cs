#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    Manifest.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/9 14:31:57
=====================================================
*/
#endregion

using System;
using UnityEngine;

[Serializable]
public class AssetRef
{
    public string name;
    public int bundle;
    public int dir;
}

[Serializable]
public class BundleRef
{
    public string name;
    public int id;
    public int[] deps;
    public long len;
    public string hash;
}

public class Manifest : ScriptableObject
{
    public string[] activeVariants = new string[0];
    public string[] dirs = new string[0];
    public AssetRef[] assets = new AssetRef[0];
    public BundleRef[] bundles = new BundleRef[0];
}