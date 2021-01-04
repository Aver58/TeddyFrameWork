#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    MainViewBase.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/14 14:55:37
=====================================================
*/
#endregion

public class MainViewBase : ViewBase
{
    public MainViewBase()
    {
        viewType = ViewType.MAIN;
    }

    protected override void AddAllListener()
    {
    }

    protected override void AddAllMessage()
    {
    }

    protected override void OnClose()
    {
    }

    protected override void OnLoaded()
    {
    }

    protected override void OnOpen(UIEventArgs args = null)
    {
    }
}