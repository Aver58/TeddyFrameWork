#region Copyright © 2018 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    ExportAssetBundle.cs
 Author:      Zeng Zhiwei
 Time:        2018\11\10 星期六 22:46:42
=====================================================
*/
#endregion

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ExportAssetBundle : Editor
{
    public static string AssetBundlePath{get{return string.Format("{0}/{1}", Application.dataPath, "Temp/data");}}

    public static void BuildBundle()
    {
        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        // Build AB到Temp目录
        BuildAssetBundle();
        GameLog.LogFormat("-->BuildBundle complete use time: {0}s", watch.ElapsedMilliseconds * 0.001f);

        watch.Reset();
    }

    public static void BuildAssetBundle()
    {
        string abPath = AssetBundlePath;
        if(!FileHelper.IsFileExist(abPath))
            FileHelper.CreateDirectory(abPath);

        //【3.更新依赖】
        //在打包的时候我们需要指定BuildAssetBundleOptions.DeterministicAssetBundle选项，
        //这个选项会为每个资源生成一个唯一的ID，当这个资源被重新打包的时候，
        //确定这个ID不会改变，包的依赖是根据这个ID来的，使用这个选项的好处是，
        //当资源需要更新时，依赖于该资源的其他资源，不需要重新打包
        //A->B->C
        //当A依赖B依赖C时，B更新，需要重新打包C，B，而A不需要动，
        //打包C的原因是，因为B依赖于C，如果不打包C，直接打包B，
        //那么C的资源就会被重复打包，而且B和C的依赖关系也会断掉
        BuildAssetBundleOptions options = BuildAssetBundleOptions.DeterministicAssetBundle;
        options |= BuildAssetBundleOptions.ChunkBasedCompression;

        BuildPipeline.BuildAssetBundles(abPath, options, EditorUserBuildSettings.activeBuildTarget);
        AssetDatabase.Refresh();
    }
}

