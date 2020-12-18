#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    SceneDefine.cs
 Author:      Zeng Zhiwei
 Time:        2020\12\18 星期五 22:14:47
=====================================================
*/
#endregion

using System;
using System.Collections.Generic;

public enum SceneID
{
    None,
    GameMain,
    TestNet,
}

public struct SceneConfig
{
    public string name;
    public string path;
    public Type viewClass;

    public SceneConfig(string name, string path, Type viewClass)
    {
        this.name = name;
        this.path = path;
        this.viewClass = viewClass;
    }
}

public static class SceneDefine
{
    public static Dictionary<SceneID, SceneConfig> SceneMap = new Dictionary<SceneID, SceneConfig>
    {
        //{SceneID.GameMain,new SceneConfig("GameMain","") }
        {SceneID.TestNet,new SceneConfig("TestNet","_Scenes/TestNet.unity",typeof(TestNetScene)) },
    };
}