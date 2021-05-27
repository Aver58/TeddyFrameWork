#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    TestView.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/12 16:50:37
=====================================================
*/
#endregion

using UnityEngine.UI;

public class TestView : MainViewBase
{
    protected override void AddAllListener()
    {
    }

    private void OpenView2()
    {
        UIModule.OpenView(ViewID.Test2);
    }

    private void ChangeScene()
    {
        SceneModule.ChangeScene(SceneID.Game);
    }
}