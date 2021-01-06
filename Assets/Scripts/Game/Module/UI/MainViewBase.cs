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

using UnityEngine;

public class MainViewBase : ViewBase
{
    public MainViewBase() { }
    public MainViewBase(GameObject go, Transform parent) : base(go, parent)
    {
        viewType = ViewType.MAIN;
    }
}