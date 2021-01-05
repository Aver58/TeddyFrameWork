#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    ViewDefine.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/12 15:33:43
=====================================================
*/
#endregion

using System;
using System.Collections.Generic;

/// <summary>
/// 界面类型
/// </summary>
public enum ViewType
{
    MAIN,//主界面
    POPUP,//弹出窗体
    FIXED,//固定弹窗
}

public struct ViewConfig
{
    public string name;
    public string path;
    public Type viewClass;

    public ViewConfig(string name, string path, Type viewClass)
    {
        this.name = name;
        this.path = path;
        this.viewClass = viewClass;
    }
}

public enum ViewID
{
    Test,
    Test2,
    MobaMainView,
}

public static class ViewDefine
{
    public static float UIPANEL_CACHE_TIME = 5f;
    public static Dictionary<ViewID, ViewConfig> ViewMapping = new Dictionary<ViewID, ViewConfig> 
    {
        { ViewID.Test,new ViewConfig("TestView","test/TestPanel",typeof(TestView))},
        { ViewID.Test2,new ViewConfig("TestView2","test/TestPanel2",typeof(TestView2))},
        { ViewID.MobaMainView,new ViewConfig("MobaMainView","battle/MobaMainPanel",typeof(MobaMainView))},
    };
}


