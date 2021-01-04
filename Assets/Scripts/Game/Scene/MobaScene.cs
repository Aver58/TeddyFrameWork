#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    MobaScene.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/28 17:47:43
=====================================================
*/
#endregion

public class MobaScene : SceneBase
{
    public override void OnEnter()
    {
        //throw new System.NotImplementedException();
        UIModule.OpenView(ViewID.MobaMainView);
    }

    public override void OnExit()
    {
        //throw new System.NotImplementedException();
    }

    public override void OnUpdate()
    {
        //throw new System.NotImplementedException();
    }

    protected override void OnLoaded()
    {
        //throw new System.NotImplementedException();
    }
}