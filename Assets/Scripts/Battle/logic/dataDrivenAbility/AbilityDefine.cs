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

// [数据驱动类技能](https://developer.valvesoftware.com/wiki/Dota_2_Workshop_Tools:zh-cn/Scripting:zh-cn/Abilities_Data_Driven:zh-cn)
// 技能事件
public enum AbilityEvent
{
    OnChannelFinish,//当持续性施法完成
    OnChannelInterrupted,//当持续性施法被中断
    OnChannelSucceeded,//当持续性施法成功
    OnOwnerSpawned,//当拥有者死亡
    OnOwnerDied,//当拥有者出生
    OnProjectileFinish,//当弹道粒子特效结束
    OnProjectileHitUnit,//当弹道粒子特效命中单位
    OnAbilityPhaseStart,
    OnAbilityPhaseCharge,
    OnSpellStart,//当技能施法开始
    OnToggleOff,//当切换为关闭状态
    OnToggleOn,//当切换为开启状态
    OnUpgrade,//当升级
}

// 技能状态
public enum AbilityState
{
    None,
    CastPoint,        //施法前摇
    Channeling,       //持续施法
    CastBackSwing,    //施法后摇
}

/// <summary>
/// 技能对象
/// </summary>
public enum MultipleTargetsTeam
{
    UNIT_TARGET_TEAM_NONE,
    UNIT_TARGET_TEAM_ENEMY,
    UNIT_TARGET_TEAM_FRIENDLY,
    UNIT_TARGET_TEAM_BOTH,
    UNIT_TARGET_TEAM_CUSTOM,
}

/// <summary>
/// 指定的类型
/// </summary>
public enum MultipleTargetsType
{
    UNIT_TARGET_NONE,//无
    UNIT_TARGET_HERO,//英雄
    UNIT_TARGET_ALL,//所有
}

//技能请求目标类型  （一样可以用 | 来指定多种类型）
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
    PASSIVE,    // 被动
    ATTACK,     // 普通攻击
    SKILL1,     // 技能1
    SKILL2,     // 技能2
    SKILL3,     // 技能3
    SKILL4,     // 技能4(optional)
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
    ABILITY_BRANCH_PHYSICAL = 1,//物理类型的技能，可被缴械等物理沉默导致技能不可释放
    ABILITY_BRANCH_MAGICAL, //法术类型的技能，可被沉默等法术沉默导致技能不可释放
}

// 伤害标记
public enum AbilityDamageFlag
{
    DAMAGE_FLAG_NONE = 1,      // 无
    DAMAGE_FLAG_CRIT = 2,      // 暴击
    DAMAGE_FLAG_LIFELINK = 4,  // 吸血
}

// 治疗
public enum AbilityHealFlag
{
    HEAL_FLAG_NONE = 1,     
    HEAL_FLAG_CRIT = 2,     // 治疗暴击
}

/// <summary>
/// 技能行为
/// 描述技能如何生效，生效后如何执行，可以配置多条行为。
/// 用来区分主动技能和被动技能。主要用于可以手动释放的大招技能的配置。
/// </summary>
public enum AbilityBehavior
{
    ABILITY_BEHAVIOR_HIDDEN = 1 << 0, //Can be owned by a unit but can't be cast and won't show up on the HUD.
    ABILITY_BEHAVIOR_PASSIVE = 1 << 1, //Cannot be cast like above but this one shows up on the ability HUD. 被动技能，AI不会选取，不能释放，自动生效
    ABILITY_BEHAVIOR_NO_TARGET = 1 << 2, //Doesn't need a target to be cast, ability fires off as soon as the button is pressed.无目标技能，不需要选取目标或者点就能释放
    ABILITY_BEHAVIOR_UNIT_TARGET = 1 << 3, //Needs a target to be cast on.单位目标技能，需要目标释放
    ABILITY_BEHAVIOR_POINT = 1 << 4, //Can be cast anywhere the mouse cursor is (if a unit is clicked it will just be cast where the unit was standing).点目标技能，释放时需要指定一个位置
    ABILITY_BEHAVIOR_AOE = 1 << 5, //Draws a radius where the ability will have effect. Kinda like POINT but with a an area of effect display.
    ABILITY_BEHAVIOR_NOT_LEARNABLE = 1 << 6, //Probably can be cast or have a casting scheme but cannot be learned (these are usually abilities that are temporary like techie's bomb detonate).
    ABILITY_BEHAVIOR_CHANNELLED = 1 << 7, //Channeled ability. If the user moves or is silenced the ability is interrupted.持续施法技能，施法者被眩晕等情况技能会被打断, 需要配合AbilityChannelTime来使用
    ABILITY_BEHAVIOR_ITEM = 1 << 8, //Ability is tied up to an item.
    ABILITY_BEHAVIOR_TOGGLE = 1 << 9, //Can be insta-toggled.
    ABILITY_BEHAVIOR_DIRECTIONAL = 1 << 10, //Has a direction from the hero, such as miranas arrow or pudge's hook.线形AOE技能，需要配合AbilityAoeLine来使用
    ABILITY_BEHAVIOR_IMMEDIATE = 1 << 11, //Can be used instantly without going into the action queue.
    ABILITY_BEHAVIOR_AUTOCAST = 1 << 12, //Can be cast automatically.
    ABILITY_BEHAVIOR_NOASSIST = 1 << 13, //Ability has no reticle assist.
    ABILITY_BEHAVIOR_AURA = 1 << 14, //Ability is an aura.  Not really used other than to tag the ability as such.
    ABILITY_BEHAVIOR_ATTACK = 1 << 15, //Is an attack and cannot hit attack-immune targets.
    ABILITY_BEHAVIOR_DONT_RESUME_MOVEMENT = 1 << 16, //Should not resume movement when it completes. Only applicable to no-target, non-immediate abilities.
    ABILITY_BEHAVIOR_ROOT_DISABLES = 1 << 17, //Cannot be used when rooted
    ABILITY_BEHAVIOR_UNRESTRICTED = 1 << 18, //Ability is allowed when commands are restricted
    ABILITY_BEHAVIOR_IGNORE_PSEUDO_QUEUE = 1 << 19, //Can be executed while stunned, casting, or force-attacking. Only applicable to toggled abilities.
    ABILITY_BEHAVIOR_IGNORE_CHANNEL = 1 << 20, //Can be executed without interrupting channels.
    ABILITY_BEHAVIOR_DONT_CANCEL_MOVEMENT = 1 << 21, //Doesn't cause certain modifiers to end, used for courier and speed burst.
    ABILITY_BEHAVIOR_DONT_ALERT_TARGET = 1 << 22, //Does not alert enemies when target-cast on them.
    ABILITY_BEHAVIOR_DONT_RESUME_ATTACK = 1 << 23, //Ability should not resume command-attacking the previous target when it completes. Only applicable to no-target, non-immediate abilities and unit-target abilities.
    ABILITY_BEHAVIOR_NORMAL_WHEN_STOLEN = 1 << 24, //Ability still uses its normal cast point when stolen.
    ABILITY_BEHAVIOR_IGNORE_BACKSWING = 1 << 25, //Ability ignores backswing pseudoqueue.
    ABILITY_BEHAVIOR_RUNE_TARGET = 1 << 26, //Targets runes.
    ABILITY_BEHAVIOR_RADIUS_AOE = 1 << 27, //圆形AOE技能，需要配合AbilityAoeRadius来使用
    ABILITY_BEHAVIOR_SECTOR_AOE = 1 << 28,//扇形AOE技能，需要配合AbilityAoeSector来使用
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

#region Action Target Values

/// <summary>
/// 技能目标
/// </summary>
public enum ActionSingTarget
{
    CASTER,
    TARGET,
    POINT,
    ATTACKER,
    UNIT,
}

/// <summary>
/// 技能目标中心点（AOE区域以中心点展开）
/// </summary>
public enum ActionMultipleTargetsCenter
{
    CASTER,
    TARGET,
    POINT,
    ATTACKER,
    PROJECTILE,
    UNIT,
}

/// <summary>
/// 伤害范围
/// </summary>
public enum AOEType
{
    Radius,//圆形半径
    Line,//线性
    Sector,//扇形
}

/// <summary>
/// Target units with specific flags or reject units with specific flag.
/// </summary>
public enum ActionMultipleTargetsFlag
{
    UNIT_TARGET_FLAG_NONE,
    UNIT_TARGET_FLAG_DEAD,
    UNIT_TARGET_FLAG_MAGIC_IMMUNE_ENEMIES,
    UNIT_TARGET_FLAG_OUT_OF_WORLD,
}

#endregion

#region Modifier

public enum ModifierAttributes
{
    MODIFIER_ATTRIBUTE_NONE,
    MODIFIER_ATTRIBUTE_MULTIPLE,
    MODIFIER_ATTRIBUTE_PERMANENT,
    MODIFIER_ATTRIBUTE_IGNORE_INVULNERABLE,//无视不可伤害
}

/// <summary>
/// 特效要附着的节点
/// </summary>
public enum ModifierEffectAttachType
{
    NO_FOLLOW_SPINE = 1,
    NO_FOLLOW_ORIGIN = 2,
    FOLLOW_ORIGIN = 3,
    FOLLOW_OVERHEAD = 4,
    FOLLOW_BIP_FOOT = 5,
    FOLLOW_BIP_HEAD = 6,
    FOLLOW_BIP_SPINE = 7,
    FOLLOW_BIP_LEFT_HAND = 8,
    FOLLOW_BIP_RIGHT_HAND = 9,
    FOLLOW_BIP_LEFT_CLAVICLE = 10,
    FOLLOW_BIP_RIGHT_CLAVICLE = 11,
    FOLLOW_BIP_WEAPON1 = 12,
    FOLLOW_BIP_WEAPON2 = 13
}

/// <summary>
/// modifier 要修改的属性
/// </summary>
public enum ModifierProperties
{
    MODIFIER_PROPERTY_PHYSICAL_ATTACK = 1,                               // 修改物理攻击
    MODIFIER_PROPERTY_PHYSICAL_DEFENCE = 2,                              // 修改物理防御
    MODIFIER_PROPERTY_PHYSICAL_PENETRATION = 3,                          // 修改物理穿透
    MODIFIER_PROPERTY_MAGIC_ATTACK = 4,                                  // 修改魔法攻击
    MODIFIER_PROPERTY_MAGIC_DEFENCE = 5,                                 // 修改魔法防御
    MODIFIER_PROPERTY_MAGIC_PENETRATION = 6,                             // 修改魔法穿透
                                                                       
    MODIFIER_PROPERTY_CRIT = 7,                                          // 修改暴击
    MODIFIER_PROPERTY_DODGE = 8,                                         // 修改闪避数值
    MODIFIER_PROPERTY_HIT_RATE = 9,                                      // 修改命中率
    MODIFIER_PROPERTY_LIFELINK_LEVEL = 10,                               // 修改吸血等级
    MODIFIER_PROPERTY_MAX_HP = 11,                                       // 修改最大HP
                                                                        
    MODIFIER_PROPERTY_STATS_STRENGTH_BONUS = 12,                         // 修改力量
    MODIFIER_PROPERTY_STATS_AGILITY_BONUS = 13,                          // 修改敏捷
    MODIFIER_PROPERTY_STATS_INTELLECT_BONUS = 14,                        // 修改智力
                                                                      
    MODIFIER_PROPERTY_INCOMING_TAKE_HEAL_VALUE_PERCENTAGE = 15,          // 修改受到的治疗效果万分比，负数降低治疗效果，正数增加治疗效果
    MODIFIER_PROPERTY_INCOMING_DEAL_HEAL_VALUE_PERCENTAGE = 16,          // 修改造成的治疗效果万分比，负数降低治疗效果，正数增加治疗效果
                                                                      
    MODIFIER_PROPERTY_INCOMING_TAKE_PHYSICAL_DAMAGE_PERCENTAGE = 17,     // 修改受到的物理伤害万分比，负数降低伤害，正数增加伤害
    MODIFIER_PROPERTY_INCOMING_DEAL_PHYSICAL_DAMAGE_PERCENTAGE = 18,     // 修改造成的物理伤害万分比，负数降低伤害，正数增加伤害
    MODIFIER_PROPERTY_INCOMING_TAKE_MAGIC_DAMAGE_PERCENTAGE = 19,        // 修改受到的魔法伤害万分比，负数降低伤害，正数增加伤害
    MODIFIER_PROPERTY_INCOMING_DEAL_MAGIC_DAMAGE_PERCENTAGE = 20,        // 修改造成的魔法伤害万分比，负数降低伤害，正数增加伤害
    MODIFIER_PROPERTY_INCOMING_TAKE_ALL_DAMAGE_PERCENTAGE = 21,          // 修改受到的所有伤害万分比，负数降低伤害，正数增加伤害
    MODIFIER_PROPERTY_INCOMING_DEAL_ALL_DAMAGE_PERCENTAGE = 22,          // 修改造成的所有伤害万分比，负数降低伤害，正数增加伤害
                                                                    
    MODIFIER_PROPERTY_PHYSICAL_SHIELD = 23,                              // 物理护盾
    MODIFIER_PROPERTY_MAGIC_SHIELD = 24,                                 // 魔法护盾
    MODIFIER_PROPERTY_SHIELD = 25,                                       // 所有伤害都可以吸收的护盾
}

/// <summary>
/// modifier 要修改的状态
/// </summary>
public enum ModifierStates
{
    //VALUE_NO_ACTION,
    //VALUE_ENABLED,
    //VALUE_DISABLED,
    MODIFIER_STATE_STUNNED = 1,                     // 眩晕
    MODIFIER_STATE_FROZEN = 2,                      // 冰冻
    MODIFIER_STATE_NIGHTMARED = 3,                  // 睡眠
    MODIFIER_STATE_SILENCED = 4,                    // 沉默（不能释放魔法技能）
    MODIFIER_STATE_DISARMED = 5,                    // 缴械（不能释放物理技能）
    MODIFIER_STATE_ROOTED = 6,                      // 缠绕状态（不可移动，不能释放物理技能）
    MODIFIER_STATE_TAUNTED = 7,                     // 嘲讽状态（强制攻击目标）
    MODIFIER_STATE_BANISH = 8,                      // 放逐
    MODIFIER_STATE_CHARM = 9,                       // 魅惑
    MODIFIER_STATE_ENERGY_RECOVERY_DISABLED = 10,   // 禁止能量回复
    MODIFIER_STATE_HIT = 11,                        // 受击
    MODIFIER_STATE_CONTROLLER_IMMUNE = 12,          // 免疫控制
}

// modifier 事件
public enum ModifierEvents
{
    OnCreated,// - The modifier has been created.
    OnDestroy,// - The modifier has been removed.
    OnAbilityExecuted,
    OnAttackStart,// - The unit this modifier is attached to has started to attack a target (when the attack animation begins, not when the autoattack projectile is created).
    OnAttackLanded,// - The unit this modifier is attached to has landed an attack on a target. "%attack_damage" is set to the damage value before mitigation. Autoattack damage is dealt after this block executes.
    OnDealDamage,// - The unit this modifier is attached to has dealt damage. "%attack_damage" is set to the damage value after mitigation.
    OnAttacked,// - The unit this modifier is attached to has been attacked.
    OnKill,//
    OnDeath,//
    OnIntervalThink,// 触发持续效果
    OnTakeDamage,// - The unit this modifier is attached to has taken damage. "%attack_damage" is set to the damage value after mitigation.
    OnOrbFire,//
    OnOrbImpact,//
    Orb,//
}

#endregion

#region 客户端

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

#endregion