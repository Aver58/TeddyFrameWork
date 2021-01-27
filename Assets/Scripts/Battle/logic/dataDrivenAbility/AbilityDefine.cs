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
    OnAbilityPhaseStart,
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
    CastPoint,      //施法前摇
    Channeling,     //持续施法
    CastBackSwing,  //施法后摇
    None,
}

// 技能对象
public enum AbilityUnitTargetTeam
{
    UNIT_TARGET_TEAM_NONE,
    UNIT_TARGET_TEAM_ENEMY,
    UNIT_TARGET_TEAM_FRIENDLY,
    UNIT_TARGET_TEAM_BOTH,
}

public enum AbilityUnitTargetType
{
    UNIT_TARGET_HERO,
}

// 技能目标
public enum AbilitySingTarget
{
    CASTER,
    TARGET,
    POINT,
}

// 技能目标中心点（AOE区域以中心点展开）
public enum AbilityUnitTargetCenter
{
    CASTER,
    TARGET,
    POINT,
    ATTACKER,
}

//技能请求目标类型
public enum AbilityRequestTargetType
{
    UNIT,       //单位
    POINT,      //点
}

// 技能类型
public enum AbilityType
{
    ABILITY_TYPE_ATTACK,
    ABILITY_TYPE_BASIC,
    ABILITY_TYPE_ULTIMATE,
}

// 技能类型
public enum AbilityCastType
{
    ATTACK,
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
    DAMAGE_TYPE_PHYSICAL,       // 物理
    DAMAGE_TYPE_MAGICAL,        //魔法
    DAMAGE_TYPE_PURE,           // 真实伤害
}

// 技能消耗类型
public enum AbilityCostType
{
    NO_COST,
    ENERGY
}

// 技能AI的目标选取条件
public enum AbilityUnitAITargetCondition
{
    UNIT_TARGET_RANDOM,
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
    ABILITY_BRANCH_PHYSICAL,//物理类型的技能，可被缴械等物理沉默导致技能不可释放
    ABILITY_BRANCH_MAGICAL, //法术类型的技能，可被沉默等法术沉默导致技能不可释放
}

// 伤害标记
public enum AbilityDamageFlag
{
    DAMAGE_FLAG_NONE = 1,      // 无
    DAMAGE_FLAG_CRIT = 2,      // 暴击
    DAMAGE_FLAG_LIFELINK = 4,  // 吸血
}


/// <summary>
/// 描述技能如何生效，生效后如何执行，可以配置多条行为。用来区分主动技能和被动技能。主要用于可以手动释放的大招技能的配置。
/// </summary>
public enum AbilityBehavior
{
    ABILITY_BEHAVIOR_PASSIVE     = 1,  //被动技能，AI不会选取，不能释放，自动生效
    ABILITY_BEHAVIOR_NO_TARGET   = 2,  //无目标技能，不需要选取目标或者点就能释放
    ABILITY_BEHAVIOR_UNIT_TARGET = 4,  //单位目标技能，需要目标释放
    ABILITY_BEHAVIOR_POINT       = 8,  //点目标技能，释放时需要指定一个位置
    ABILITY_BEHAVIOR_CHANNELLED  = 16, //持续施法技能，施法者被眩晕等情况技能会被打断, 需要配合AbilityChannelTime来使用
    ABILITY_BEHAVIOR_RADIUS_AOE  = 32, //圆形AOE技能，需要配合AbilityAoeRadius来使用
    ABILITY_BEHAVIOR_LINE_AOE    = 64, //线形AOE技能，需要配合AbilityAoeLine来使用
    ABILITY_BEHAVIOR_SECTOR_AOE  = 128,//扇形AOE技能，需要配合AbilityAoeSector来使用
}

/// <summary>
/// 技能指示器类型
/// </summary>
public enum AbilityIndicatorType
{
    CIRCLE_AREA,
    LINE_AREA,
    SECTOR60_AREA,
    SECTOR90_AREA,
    SECTOR120_AREA,
    RANGE_AREA,
    SEGMENT,
    SEGMENT_AREA
}

/// <summary>
/// 技能数值来源
/// </summary>
public enum AbilityValueSourceType
{
    SOURCE_TYPE_PHYSICAL_ATTACK,
    SOURCE_TYPE_MAGICAL_ATTACK,
    SOURCE_TYPE_CASTER_MAX_HP,
    SOURCE_TYPE_CASTER_CURRENT_HP,
    SOURCE_TYPE_CASTER_LOST_HP,
    SOURCE_TYPE_TARGET_MAX_HP,
    SOURCE_TYPE_TARGET_CURRENT_HP,
    SOURCE_TYPE_TARGET_LOST_HP,
}