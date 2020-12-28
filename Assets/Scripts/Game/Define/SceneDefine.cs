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

public enum SceneID
{
    None,
    Test,
    GameMain,
    Game,
    TestNet,
    Moba,
}

public static class SceneDefine
{
    public static Dictionary<SceneID, SceneConfig> SceneMap = new Dictionary<SceneID, SceneConfig>
    {
        //{SceneID.Test,new SceneConfig("Test","Assets/_Scenes/Test.unity"),typeof(TestScene) },
        //{SceneID.GameMain,new SceneConfig("GameMain","") }
        {SceneID.TestNet,new SceneConfig("TestNet","Assets/_Scenes/TestNet.unity",typeof(TestNetScene)) },
        {SceneID.Game,new SceneConfig("Game","Assets/_Scenes/Game.unity",typeof(GameScene)) },
        {SceneID.Moba,new SceneConfig("Moba","Assets/_Scenes/MobaScene.unity",typeof(MobaScene)) },
    };
}