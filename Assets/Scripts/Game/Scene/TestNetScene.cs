#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    TestNetScene.cs
 Author:      Zeng Zhiwei
 Time:        2020\12\18 星期五 22:47:29
=====================================================
*/
#endregion

using UnityEngine;

public class TestNetScene : SceneBase
{
    public override void OnEnter()
    {
        GameLog.Log(panelName+"OnEnter");
    }

    public override void OnExit()
    {
        GameLog.Log(panelName + "OnExit");
    }

    public override void OnUpdate()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnLoaded()
    {
        throw new System.NotImplementedException();
    }
}