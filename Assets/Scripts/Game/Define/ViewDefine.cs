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

public enum ViewType
{
    MAIN = 1,
    POPUP,
}

public enum ViewID
{
    Test = 1,
}

public static class ViewDefine
{
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

    public static Dictionary<ViewID, ViewConfig> ViewMapping = new Dictionary<ViewID, ViewConfig> 
    {
        { ViewID.Test,new ViewConfig("TestView","test.TestPanel",typeof(TestView))},
    };
}
