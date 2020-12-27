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
}

public class HeorTurnEventArgs : EventArgs
{
    public int id;
    public Vector2 forward;
}

public class HeorHPUpdateEventArgs : EventArgs
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

public class HeorChangeStateEventArgs : EventArgs
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

public class BattleActorCreateEventArgs : EventArgs
{
    public HeroActor heroActor;
    public bool isFriend;

    public BattleActorCreateEventArgs(HeroActor heroActor, bool isFriend)
    {
        this.heroActor = heroActor;
        this.isFriend = isFriend;
    }
}
#endregion
