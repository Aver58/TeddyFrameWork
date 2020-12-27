#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    GameMsgDef.cs
 Author:      Zeng Zhiwei
 Time:        2020/5/24 19:21:56
=====================================================
*/
#endregion

public enum GameMsgDef
{
    Hero_MoveTo = 1,    //英雄移动到某个点
    Hero_TurnTo2D,      //英雄旋转到某个点
    Hero_ChangeState,   //英雄状态切换
    BattleEntity_Created,   //一个角色创建
    BattleEntity_HP_Updated,   //一个角色创建
}