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

    protected override void AddListeners()
    {
        throw new System.NotImplementedException();
    }

    protected override void AddMessages()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnClose()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnLoaded()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnOpen(UIEventArgs args = null)
    {
        throw new System.NotImplementedException();
    }
}