#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    GameEventArgs.cs
 Author:      Zeng Zhiwei
 Time:        2020/5/25 13:37:57
=====================================================
*/
#endregion

using System;
using UnityEngine;

public class GameEventArgs : EventArgs
{
    public int intParam;
    public float floatParam;
    public string stringParam;
    public Vector2 vector2Param;
    public object objectParam;
}

#region 业务需要的参数
public class HeorMoveEventArgs : EventArgs
{
    public int id;
    public Vector3 targetPos;
    public Vector3 forward;

    public HeorMoveEventArgs(int id, Vector3 targetPos, Vector3 forward)
    {
        this.id = id;
        this.targetPos = targetPos;
        this.forward = forward;
    }
}

#endregion
