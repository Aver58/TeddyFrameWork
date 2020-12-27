#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    AbilityDefine.cs
 Author:      Zeng Zhiwei
 Time:        2020/6/20 9:37:19
=====================================================
*/
#endregion

// 技能事件
public enum AbilityEvent
{
    OnAbilityPhaseStart = 1,
    OnAbilityPhaseCharge,
    OnSpellStart,
    OnChannelFinish,
    OnChannelInterrupted,
    OnChannelSucceeded,
    OnOwnerSpawned,
    OnOwnerDied,
    OnProjectileFinish,
    OnProjectileHitUnit,
}

// 技能状态
public enum AbilityState
{
    CastPoint = 0,  //施法前摇
    Channeling,     //持续施法
    CastBackSwing,  //施法后摇
    None,
}

// 技能对象
public enum AbilityUnitTargetTeam
{
    UNIT_TARGET_TEAM_NONE = 1,
    UNIT_TARGET_TEAM_ENEMY,
    UNIT_TARGET_TEAM_FRIENDLY,
    UNIT_TARGET_TEAM_BOTH,
}

// 技能目标
public enum AbilitySingTarget
{
    CASTER = 1,
    TARGET,
    POINT,
}

//技能请求目标类型
public enum AbilityRequestTargetType
{
    UNIT = 1,   //单位
    POINT,      //点
}

// 技能类型
public enum AbilityType
{
    ABILITY_TYPE_ATTACK = 1,
    ABILITY_TYPE_BASIC,
    ABILITY_TYPE_ULTIMATE,
}

// 技能类型
public enum AbilityCastType
{
    ATTACK = 1,
    SKILL1,
    SKILL2,
    SKILL3,
}

// 伤害范围
public enum AbilityAreaDamageType
{
    Radius = 1,//圆形半径
    Line,//线性
    Sector,//扇形
}

// 伤害类型
public enum AbilityDamageType
{
    DAMAGE_TYPE_PHYSICAL = 1,   // 物理
    DAMAGE_TYPE_MAGICAL,        //魔法
    DAMAGE_TYPE_PURE,           // 真实伤害
}

// 技能消耗类型
public enum AbilityCostType
{
    NO_COST = 1,
    ENERGY = 2
}

// 技能AI的目标选取条件
public enum AbilityUnitAITargetCondition
{
    UNIT_TARGET_RANDOM = 1,
    UNIT_TARGET_RANGE_MIN ,
    UNIT_TARGET_RANGE_MAX,
    UNIT_TARGET_HP_MIN ,
    UNIT_TARGET_HP_MAX ,
    UNIT_TARGET_HP_PERCENT_MIN,
    UNIT_TARGET_HP_PERCENT_MAX,
    UNIT_TARGET_ENERGY_MIN,
    UNIT_TARGET_ENERGY_MAX,
}

// 技能分支
public enum AbilityBranch
{
    ABILITY_BRANCH_PHYSICAL = 1,
    ABILITY_BRANCH_MAGICAL = 2
}

// 伤害标记
public enum AbilityDamageFlag
{
    DAMAGE_FLAG_NONE = 1,      // 无
    DAMAGE_FLAG_CRIT = 2,      // 暴击
    DAMAGE_FLAG_LIFELINK = 4,      // 吸血
}

// 技能行为
public enum AbilityBehavior
{
    ABILITY_BEHAVIOR_PASSIVE     = 1,
    ABILITY_BEHAVIOR_NO_TARGET   = 2,
    ABILITY_BEHAVIOR_UNIT_TARGET = 4,
    ABILITY_BEHAVIOR_POINT       = 8,
    ABILITY_BEHAVIOR_CHANNELLED  = 16,
    ABILITY_BEHAVIOR_RADIUS_AOE  = 32,
    ABILITY_BEHAVIOR_LINE_AOE    = 64,
    ABILITY_BEHAVIOR_SECTOR_AOE  = 128,
}

/// <summary>
/// 技能数值来源
/// </summary>
public enum AbilityValueSourceType
{
    SOURCE_TYPE_PHYSICAL_ATTACK = 1,
    SOURCE_TYPE_MAGICAL_ATTACK = 2,
    SOURCE_TYPE_CASTER_MAX_HP = 3,
    SOURCE_TYPE_CASTER_CURRENT_HP = 4,
    SOURCE_TYPE_CASTER_LOST_HP = 5,
    SOURCE_TYPE_TARGET_MAX_HP = 6,
    SOURCE_TYPE_TARGET_CURRENT_HP = 7,
    SOURCE_TYPE_TARGET_LOST_HP = 8,
}