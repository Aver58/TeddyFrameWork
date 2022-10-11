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
    Hero_TurnTo3D,      //英雄旋转到某个点
    Hero_ChangeState,   //英雄状态切换
    BattleActor_Created,   //一个角色实例创建
    PlayerActor_Created,   //客户端玩家实例创建
    BattleEntity_HP_Updated,   //一个角色创建
    Hero_Cast_Ability,          //一个角色释放技能

    DAMAGE_EFFECT_NOTICED,          //傷害特效通知

    OnProjectileActorCreated, // 子弹实例创建
    OnProjectileActorDestroy, // 子弹实例销毁
    OnProjectileActorMoveTo, // 子弹移动
}