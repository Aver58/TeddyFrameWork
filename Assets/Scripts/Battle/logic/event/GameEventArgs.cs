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

using UnityEngine;

/// <summary>
/// 消息传递的结构体放这
/// </summary>
public class EventArgs
{
    public static readonly EventArgs Empty;
}

#region 业务需要的参数

public struct HeorMoveEventArgs 
{
    public int id;
    public Vector3 targetPos;
    public Vector3 forward;
}

public struct HeorTurnEventArgs
{
    public int id;
    public Vector2 forward;
}

public struct HeorHPUpdateEventArgs
{
    public int id;
    public int curHp;
    public int maxHp;

    public HeorHPUpdateEventArgs(int id, int curHp, int maxHp)
    {
        this.id = id;
        this.curHp = curHp;
        this.maxHp = maxHp;
    }
}

public struct HeorChangeStateEventArgs
{
    public int id;
    public HeroState heroState;
    public string skillName;
    public bool isSkipCastPoint;

    public HeorChangeStateEventArgs(int id, HeroState heroState, string skillName = null, bool isSkipCastPoint = false)
    {
        this.id = id;
        this.heroState = heroState;
        this.skillName = skillName;
        this.isSkipCastPoint = isSkipCastPoint;
    }
}

#endregion
