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
        UIModule.OpenView(ViewID.MobaMainView);

        MobaBussiness.Instance.Init();
    }

    public override void OnExit()
    {
    }

    public override void OnUpdate()
    {
    }

    protected override void OnLoaded()
    {
    }
}