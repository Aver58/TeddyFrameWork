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
    OnSpellStart,//当技能施法开始
    OnToggleOn,//当切换为开启状态
    OnToggleOff,//当切换为关闭状态
    OnChannelFinish,//当持续性施法完成
    OnChannelInterrupted,//当持续性施法被中断
    OnChannelSucceeded,//当持续性施法成功
    OnOwnerSpawned,//当拥有者死亡
    OnOwnerDied,//当拥有者出生
    OnProjectileFinish,//当弹道粒子特效结束
    OnProjectileHitUnit,//当弹道粒子特效命中单位
    OnAbilityPhaseCharge,
    OnEquip,//道具被捡起
    OnUnequip,//道具离开物品栏
    OnUpgrade,//从用户界面升级此技能
    OnAbilityPhaseStart,//技能开始施法（单位转向目标前）
}

/// <summary>
/// 施法状态
/// </summary>
public enum AbilityState
{
    None,
    CastPoint,        //施法前摇
    Channeling,       //持续施法
    CastBackSwing,    //施法后摇
}

#region Target

/// <summary>
/// 队伍
/// </summary>
public enum AbilityUnitTargetTeam
{
    UNIT_TARGET_TEAM_NONE,
    UNIT_TARGET_TEAM_ENEMY,
    UNIT_TARGET_TEAM_FRIENDLY,
    UNIT_TARGET_TEAM_BOTH,
    UNIT_TARGET_TEAM_CUSTOM,
}

/// <summary>
/// 类型
/// </summary>
public enum AbilityUnitTargetType
{
    DOTA_UNIT_TARGET_HERO,//英雄
    DOTA_UNIT_TARGET_ALL,//任意，包括隐藏的实体
    DOTA_UNIT_TARGET_BASIC,//基本单位, 包括召唤单位
    DOTA_UNIT_TARGET_MECHANICAL,//npc_dota_creep_siege 机械单位（投石车） DOTA_NPC_UNIT_RELATIONSHIP_TYPE_SIEGE
    DOTA_UNIT_TARGET_BUILDING,//npc_dota_tower, npc_dota_building 塔和建筑 DOTA_NPC_UNIT_RELATIONSHIP_TYPE_BUILDING
    DOTA_UNIT_TARGET_TREE,//ent_dota_tree 树 例子: 吃树, 补刀斧
    DOTA_UNIT_TARGET_CREEP,//npc_dota_creature, npc_dota_creep 与BASIC类似但是可能不包括一些召唤单位 例子: 骨弓死亡契约
    DOTA_UNIT_TARGET_COURIER,//npc_dota_courier, npc_dota_flying_courier 信使和飞行信使 DOTA_NPC_UNIT_RELATIONSHIP_TYPE_COURIER
    DOTA_UNIT_TARGET_NONE,//没有
    DOTA_UNIT_TARGET_OTHER,//任何前面不包括的单位
    DOTA_UNIT_TARGET_CUSTOM,//未开放? 例子: 水人复制, TB灵魂隔断, 谜团恶魔转化, 艾欧链接, 小狗感染...
}

/// <summary>
/// 标签允许对默认被忽略的目标单位 (例如魔法免疫敌人)施法, 或者忽略特定的单位类型 (例如远古单位和魔免友军)来允许对其施法.
/// </summary>
public enum AbilityUnitTargetFlags
{
    DOTA_UNIT_TARGET_FLAG_NONE,//缺省默认值
    DOTA_UNIT_TARGET_FLAG_DEAD,//死亡单位忽略
    DOTA_UNIT_TARGET_FLAG_MELEE_ONLY,//拥有攻击类型DOTA_UNIT_CAP_MELEE_ATTACK的单位
    DOTA_UNIT_TARGET_FLAG_RANGED_ONLY,//拥有攻击类型DOTA_UNIT_CAP_RANGED_ATTACK的单位
    DOTA_UNIT_TARGET_FLAG_MANA_ONLY,//在npc_unit中拥有魔法没有"StatusMana" "0"的单位
    DOTA_UNIT_TARGET_FLAG_CHECK_DISABLE_HELP,//禁用帮助的单位 尚不确定数据驱动技能如何使用？
    DOTA_UNIT_TARGET_FLAG_NO_INVIS,//忽略拥有MODIFIER_STATE_INVISIBLE的不可见单位
    DOTA_UNIT_TARGET_FLAG_MAGIC_IMMUNE_ENEMIES,//指向拥有MODIFIER_STATE_MAGIC_IMMUNE （魔法免疫）的敌人单位 例子: 根须缠绕, 淘汰之刃, 原始咆哮...
    DOTA_UNIT_TARGET_FLAG_NOT_MAGIC_IMMUNE_ALLIES,//忽略拥有MODIFIER_STATE_MAGIC_IMMUNE（魔法免疫）的友军 例子: 痛苦之源噩梦
    DOTA_UNIT_TARGET_FLAG_NOT_ATTACK_IMMUNE,//拥有MODIFIER_STATE_ATTACK_IMMUNE的单位（攻击免疫单位）
    DOTA_UNIT_TARGET_FLAG_FOW_VISIBLE,//单位离开视野时打断 例子: 魔法汲取, 生命汲取
    DOTA_UNIT_TARGET_FLAG_INVULNERABLE,//拥有MODIFIER_STATE_INVULNERABLE的单位（无敌单位） 例子: 暗杀, 召回, 巨力重击...
    DOTA_UNIT_TARGET_FLAG_NOT_ANCIENTS,//忽略单位，使用标签"IsAncient" "1" 例子: 麦达斯之手
    DOTA_UNIT_TARGET_FLAG_NOT_CREEP_HERO,//忽略单位，使用标签"ConsideredHero" "1" 例子: 星体禁锢, 崩裂禁锢, 灵魂隔断
    DOTA_UNIT_TARGET_FLAG_NOT_DOMINATED,//拥有MODIFIER_STATE_DOMINATED的单位（被支配的单位）
    DOTA_UNIT_TARGET_FLAG_NOT_ILLUSIONS,//拥有MODIFIER_PROPERTY_IS_ILLUSION的单位（幻象单位）
    DOTA_UNIT_TARGET_FLAG_NOT_NIGHTMARED,//拥有MODIFIER_STATE_NIGHTMARED的单位（噩梦中单位）
    DOTA_UNIT_TARGET_FLAG_NOT_SUMMONED,//通过SpawnUnit Action创建的单位
    DOTA_UNIT_TARGET_FLAG_OUT_OF_WORLD,//拥有MODIFIER_STATE_OUT_OF_GAME的单位（离开游戏的单位）
    DOTA_UNIT_TARGET_FLAG_PLAYER_CONTROLLED,//玩家控制的单位，接近Lua IsControllableByAnyPlayer()
}

#endregion

#region Action

public enum AbilityAction {
    AddAbility,//添加技能 目标, 技能名 Target, AbilityName
    ActOnTargets,//目标动作 目标,动作 Target, Action
    ApplyModifier,//	应用Modifier 目标, Modifier名称, 持续时间 Target, ModifierName, Duration
    ApplyMotionController,//	应用移动控制器 脚本文件,水平控制函数,垂直控制函数,测试重力函数 ScriptFile, HorizontalControlFunction, VerticalControlFunction, TestGravityFunc
    AttachEffect,//当在modifier中使用时, AttachEffect将会在modifier被摧毁后自动停止例子特效,而FireEffect不会,所以如果你在modifier中使用FireEffect添加一个无限持续的粒子特,modifier被摧毁后效果依然会持续
    //	附着效果 效果名称, 效果附加类型, 目标, 目标点, 控制点, 控制点实体, 目标点, 效果范围, 效果持续时间比例, 效果生命时间比例 ,效果颜色A, 效果颜色B, 效果透明度比例
    //EffectName, EffectAttachType, Target, TargetPoint, ControlPoints,ControlPointEntities, TargetPoint, EffectRadius, EffectDurationScale, EffectLifeDurationScale ,EffectColorA, EffectColorB, EffectAlphaScale 
    Blink,//闪烁 目标 Target
    CleaveAttack,//	范围攻击 范围效果,范围百分比,范围半径 Cleave Effect, CleavePercent, CleaveRadius
    CreateBonusAttack,//	创建奖励攻击 目标 Target
    CreateThinker,//	创建Thinker 目标,Modifier名称 Target, ModifierName
    CreateThinkerWall,//	创建ThinkerWall 目标,Modifier名称,宽度,长度,旋转 Target, ModifierName, Width, Length, Rotation
    Damage,//	伤害 
    // 目标,类型,最小伤害/最大伤害，伤害，基于当前生命百分比伤害，基于最大生命百分比伤害
    // Target, Type, MinDamage/MaxDamage, Damage, CurrentHealthPercentBasedDamage, MaxHealthPercentBasedDamage
    DelayedAction,//	延迟动作 延迟,动作 Delay, Action
    DestroyTrees,//	破坏树木 目标,范围 Target, Radius
    FireEffect,//	播放特效
    // 效果名称, 效果附加类型, 目标, 目标点, 控制点, 目标点, 效果范围, 效果持续时间比例, 效果生命时间比例 ,效果颜色A, 效果颜色B, 效果透明度比例
    // EffectName, EffectAttachType, Target, TargetPoint, ControlPoints, TargetPoint, EffectRadius, EffectDurationScale, EffectLifeDurationScale ,EffectColorA, EffectColorB, EffectAlphaScale
    FireSound,//	播放声音 效果名称,目标 EffectName, Target
    GrantXPGold,//	给予经验金钱 目标,经验数量,金币数量,可靠金币,均匀分配 Target, XPAmount, GoldAmount, ReliableGold, SplitEvenly
    Heal,//	治疗 治疗量,目标 HealAmount, Target
    IsCasterAlive,//	施法者生存 成功时,失败时 OnSuccess, OnFailure
    Knockback,//	击退 目标,中心,持续时间,距离,高度,固定距离,眩晕 Target, Center, Duration, Distance, Height, IsFixedDistance, ShouldStun
    LevelUpAbility,//	升级技能 目标,技能名称 Target, AbilityName
    Lifesteal,//	吸血 目标,吸血比例 Target, LifestealPercent
    LinearProjectile,//	线性投射物
    // 目标,效果名称,移动速度,开始范围,结束范围,固定距离,开始位置,目标队伍,目标类型,目标标签,前方锥形,提供视野,视觉范围
    // Target, EffectName, MoveSpeed, StartRadius, EndRadius, FixedDistance, StartPosition, TargetTeams, TargetTypes, TargetFlags, HasFrontalCone, ProvidesVision, VisionRadius
    MoveUnit,//	移动单位 目标,移向目标 Target, MoveToTarget
    Random,//	随机 几率,伪随机,成功时,失败时 Chance, PseudoRandom, OnSuccess, OnFailure
    RemoveAbility,//	移除技能 目标,技能名称 Target, AbilityName
    RemoveModifier,//	移除Modifier 目标,Modifier名称 Target, ModifierName
    RemoveUnit,//	移除单位 目标 Target
    ReplaceUnit	,//替换单位 单位名称,目标 UnitName,Target
    Rotate,//	旋转 目标,俯仰角偏航角翻滚角 Target, PitchYawRoll
    RunScript,//	运行脚本 目标,脚本文件,函数,额外参数 Target, ScriptFile, Function, Extra Parameters
    SpawnUnit,//	产生单位 目标名称,目标数量,目标上限,产生范围,持续时间,目标,死亡金钱,死亡经验 UnitName, UnitCount, UnitLimit, SpawnRadius, Duration, Target, GrantsGold, GrantsXP
    Stun,//	眩晕 目标,持续时间 Target, Duration
    SpendMana,//	花费魔法值 魔法值 Mana
    TrackingProjectile,//	追踪投射物 目标, 效果名称, 能否躲避, 提供视野, 视野范围, 移动速度, 源附着点 Target, EffectName, Dodgeable, ProvidesVision, VisionRadius, MoveSpeed, SourceAttachment
}

#endregion

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

/// <summary>
/// 伤害类型
/// </summary>
public enum AbilityUnitDamageType
{
    DAMAGE_TYPE_PHYSICAL,       // 物理伤害会被护甲和格挡（圆盾）减少
    DAMAGE_TYPE_MAGICAL,        // 魔法伤害会被魔抗减少
    DAMAGE_TYPE_PURE,           // 纯粹伤害不会被任何护甲或魔抗减少
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
/// 描述技能如何生效，生效后如何执行，可以配置多条行为。使用空格和 | 符号分割例子（注意空格！）
/// 用来区分主动技能和被动技能。主要用于可以手动释放的大招技能的配置。
/// </summary>
public enum AbilityBehavior
{
    ABILITY_BEHAVIOR_NO_TARGET  = 1, // 无目标技能，不需要选择目标释放，按下技能按钮即可释放
    ABILITY_BEHAVIOR_UNIT_TARGET, // 单位目标技能，需要目标释放，需要AbilityUnitTargetTeam和AbilityUnitTargetType，参见
    ABILITY_BEHAVIOR_POINT, // 点目标技能，可以对鼠标指向的任意位置释放，如果点击单位也只是对其位置释放
    ABILITY_BEHAVIOR_PASSIVE, // 被动技能，AI不会选取，不能释放，自动生效 Cannot be cast like above but this one shows up on the ability HUD.
    ABILITY_BEHAVIOR_CHANNELLED, // 持续施法技能，施法者被眩晕等情况技能会被打断, 需要配合AbilityChannelTime来使用 Channeled ability. If the user moves or is silenced the ability is interrupted.
    ABILITY_BEHAVIOR_TOGGLE, // 开关技能，可以开关 Can be insta-toggled.
    ABILITY_BEHAVIOR_AURA, // 光环技能，并没有实际作用而只是作为一个标签 Ability is an aura.  Not really used other than to tag the ability as such.
    ABILITY_BEHAVIOR_AUTOCAST, // 自动施法，可以自动施法，通常如果不是一个 ATTACK技能的话本身并不工作 Can be cast automatically.
    ABILITY_BEHAVIOR_HIDDEN, // 隐藏，不能释放并且不在HUD上显示 Can be owned by a unit but can't be cast and won't show up on the HUD.
    ABILITY_BEHAVIOR_AOE, // AOE技能，会显示技能将要影响的范围，类似点目标技能，需要配合AoERadius来使用 Draws a radius where the ability will have effect. Kinda like POINT but with a an area of effect display.
    ABILITY_BEHAVIOR_NOT_LEARNABLE, // 不可学习技能，不能通过点击HUD学习，例如卡尔的技能 Probably can be cast or have a casting scheme but cannot be learned (these are usually abilities that are temporary like techie's bomb detonate).
    ABILITY_BEHAVIOR_ITEM, // 道具技能，技能与道具绑定，并不需要使用，游戏会在自动将此属性添加给任何基类为"item_datadriven"的技能 Ability is tied up to an item.
    ABILITY_BEHAVIOR_DIRECTIONAL, // 方向技能，拥有一个从英雄出发的方向，例如白虎的箭和屠夫的钩子 Has a direction from the hero, such as miranas arrow or pudge's hook.
    ABILITY_BEHAVIOR_IMMEDIATE, // 立即释放技能，立即释放而不进入操作序列 Can be used instantly without going into the action queue.
    ABILITY_BEHAVIOR_NOASSIST, //技能没有辅助刻度 ？ Ability has no reticle assist.
    ABILITY_BEHAVIOR_ATTACK, // ATTACK（攻击）技能，不能攻击不能被攻击的单位 Is an attack and cannot hit attack-immune targets.
    ABILITY_BEHAVIOR_ROOT_DISABLES, // 被定身时无法使用 Cannot be used when rooted
    ABILITY_BEHAVIOR_UNRESTRICTED, // 指令被限制时依然能够使用，例如食尸鬼大招 Ability is allowed when commands are restricted
    ABILITY_BEHAVIOR_DONT_ALERT_TARGET, // 释放在敌人身上时不会警告他们，例如白牛的冲 Does not alert enemies when target-cast on them.
    ABILITY_BEHAVIOR_DONT_RESUME_MOVEMENT, // 完成施法后不会恢复移动，只能对无目标、非立即技能生效 Should not resume movement when it completes. Only applicable to no-target, non-immediate abilities.
    ABILITY_BEHAVIOR_DONT_RESUME_ATTACK, // 完成施法后不会继续继续攻击之前的目标，只对无目标，非立即释放和单位目标技能生效 Ability should not resume command-attacking the previous target when it completes. Only applicable to no-target, non-immediate abilities and unit-target abilities.
    ABILITY_BEHAVIOR_NORMAL_WHEN_STOLEN, // 被偷窃时依然使用其默认施法前摇，例如地卜师的忽悠和先知的传送 Ability still uses its normal cast point when stolen.
    ABILITY_BEHAVIOR_IGNORE_BACKSWING, // 无视施法后摇 Ability ignores backswing pseudoqueue.
    ABILITY_BEHAVIOR_IGNORE_PSEUDO_QUEUE, // 在被眩晕、施法时或者强制攻击时都能执行，只对开关技能有效，例如变形 Can be executed while stunned, casting, or force-attacking. Only applicable to toggled abilities.
    ABILITY_BEHAVIOR_RUNE_TARGET, // 可对符文释放 Targets runes.
    ABILITY_BEHAVIOR_IGNORE_CHANNEL, //Can be executed without interrupting channels.
    ABILITY_BEHAVIOR_DONT_CANCEL_MOVEMENT, //Doesn't cause certain modifiers to end, used for courier and speed burst.
    ABILITY_BEHAVIOR_RADIUS_AOE, //圆形AOE技能，需要配合AbilityAoeRadius来使用
    ABILITY_BEHAVIOR_SECTOR_AOE,//扇形AOE技能，需要配合AbilityAoeSector来使用
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

#endregion

#region Modifier

public enum ModifierAttributes
{
    MODIFIER_ATTRIBUTE_NONE,
    MODIFIER_ATTRIBUTE_MULTIPLE,//同一个modifier可以存在多个实例，而不会覆盖
    MODIFIER_ATTRIBUTE_PERMANENT,//死亡保持
    MODIFIER_ATTRIBUTE_IGNORE_INVULNERABLE,//无视不可伤害,将会在无敌单位(MODIFIER_STATE_INVULNERABLE )上保持 想要对一个无敌单位应用你需要使用此属性，并在"Target"中使用"Flag" "DOTA_UNIT_TARGET_FLAG_INVULNERABLE"
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
    OnCreated,// modifier创建时 - The modifier has been created.
    OnDestroy,// modifier移除时 - The modifier has been removed.
    OnIntervalThink,//	每隔ThinkInterval秒
    OnAttack,//	附加这个modifier的单位完成攻击时
    OnAttacked,//	附加这个modifier的单位被攻击时，攻击结束时触发. 
    OnAttackStart,//	附加这个modifier的单位开始攻击目标时 是在攻击动画开始时，而不是投射物创建时
    OnAttackLanded,//	附加这个modifier的单位击中目标时
    OnAttackFailed,//	单位攻击丢失时
    OnAttackAllied,//	攻击队友单位时
    OnDealDamage,//	附加这个modifier的单位造成伤害时
    OnTakeDamage,//	附加这个modifier的单位受到伤害时"%attack_damage"是实际收到伤害值.
    OnDeath,//	附加这个modifier的单位死亡
    OnKill,//	单位杀死任意东西时
    OnHeroKill,//	单位杀死英雄时
    OnRespawn,//	经过复活时间，单位重生时
    Orb,//	使用法球每次攻击时
    OnOrbFire,//	法球的OnAttackStart
    OnOrbImpact,//	法球的OnAttackLanded
    OnAbilityExecuted,//	附加这个modifier的单位使用任意技能(包括物品)
    OnAbilityStart,//	附加这个modifier的单位开始技能 与OnSpellStart相同，但是是一个Modifier Event
    OnAbilityEndChannel,//	附加这个modifier的单位因为任何原因结束持续施法时
    OnHealReceived,//	单位因为任何原因获取生命值时，满生命值也会触发
    OnHealthGained,//	从外源获取生命值时
    OnManaGained,//	获取魔法值时，满魔法也会触发.
    OnSpentMana,//	单位花费魔法值时
    OnOrder,//	当移动/施法/驻守/提示命令发布怒时
    OnUnitMoved,//	移动时触发
    OnTeleported,//	完成传送时触发
    OnTeleporting,//	开始传送时触发
    OnProjectileDodge,//	附加这个modifier的单位躲避投射物时
    OnStateChanged,//	(可能) 单位获取modifier时
}

#endregion

#region Client

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