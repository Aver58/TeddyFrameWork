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
// https://moddota.com/api/#!/vscripts
using System;

// 技能事件
public enum AbilityEvent {
    OnCreated,
    OnSpellStart,//当技能施法开始
    OnChannelFinish,//当持续性施法完成
    OnChannelInterrupted,//当持续性施法被中断
    OnChannelSucceeded,//当持续性施法成功
    OnProjectileHitUnit,//当弹道粒子特效命中单位 Adding the KV pair "DeleteOnHit" "0" in this block will cause the projectile to not disappear when it hits a unit.
    OnProjectileFinish,//当弹道粒子特效结束
    OnProjectileDodge,//当弹道粒子特效被闪避
    OnAbilityPhaseStart,//技能开始施法（单位转向目标前） Triggers when the ability is cast (before the unit turns toward the target)
    OnAbilityPhaseCharge,
    // 拓展，todo实现
    OnUpgrade,//从用户界面升级此技能
    OnEquip,//道具被捡起
    OnUnequip,//道具离开物品栏
    OnOwnerSpawned,//当拥有者死亡
    OnOwnerDied,//当拥有者出生
    OnRespawn,
    OnToggleOn,//当切换为开启状态
    OnToggleOff,//当切换为关闭状态
    OnManaGained,
    OnUnitMoved,
    OnTeleported,
    OnTeleporting,
}

/// <summary>
/// 施法状态
/// </summary>
public enum AbilityState {
    None,
    CastPoint,        //施法前摇
    Channeling,       //持续施法
    CastBackSwing,    //施法后摇
}

#region Action

public enum AbilityAction {
    AddAbility,//添加技能 目标, 技能名 Target, AbilityName
    ActOnTargets,//目标动作 目标,动作 Target, Action
    ApplyModifier,//	应用Modifier 目标, Modifier名称, 持续时间 Target, ModifierName, Duration
    ApplyMotionController,//	应用移动控制器 脚本文件,水平控制函数,垂直控制函数,测试重力函数 ScriptFile, HorizontalControlFunction, VerticalControlFunction, TestGravityFunc
    AttachEffect,//当在modifier中使用时, AttachEffect将会在modifier被摧毁后自动停止例子特效,而FireEffect不会,所以如果你在modifier中使用FireEffect添加一个无限持续的粒子特,modifier被摧毁后效果依然会持续
    //附着效果 效果名称, 效果附加类型, 目标, 目标点, 控制点, 控制点实体, 目标点, 效果范围, 效果持续时间比例, 效果生命时间比例 ,效果颜色A, 效果颜色B, 效果透明度比例
    //EffectName, EffectAttachType, Target, TargetPoint, ControlPoints,ControlPointEntities, TargetPoint, EffectRadius, EffectDurationScale, EffectLifeDurationScale ,EffectColorA, EffectColorB, EffectAlphaScale 
    Blink,//闪烁 目标 Target
    CleaveAttack,//	范围攻击 范围效果,范围百分比,范围半径 Cleave Effect, CleavePercent, CleaveRadius
    CreateBonusAttack,//	创建奖励攻击 目标 Target
    CreateThinker,//	创建Thinker 目标,Modifier名称 Target, ModifierName
    CreateThinkerWall,//	创建ThinkerWall 目标,Modifier名称,宽度,长度,旋转 Target, ModifierName, Width, Length, Rotation
    Damage,//	伤害
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
    TrackingProjectile,//	追踪投射物 目标, 效果名称, 能否躲避, 提供视野, 视野范围, 移动速度, 源附着点 Target, EffectName, Dodgeable, ProvidesVision, VisionRadius, MoveSpeed, SourceAttachment
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
}

#endregion

//技能请求目标类型  （一样可以用 | 来指定多种类型）
public enum AbilityRequestTargetType {
    UNIT,       //单位
    POINT,      //点
}

// 技能类型
public enum AbilityType {
    ABILITY_TYPE_ATTACK,
    ABILITY_TYPE_BASIC,
    ABILITY_TYPE_ULTIMATE,
}

// 技能类型
public enum AbilityCastType {
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
public enum AbilityUnitDamageType {
    DAMAGE_TYPE_PHYSICAL,       // 物理伤害会被护甲和格挡（圆盾）减少
    DAMAGE_TYPE_MAGICAL,        // 魔法伤害会被魔抗减少
    DAMAGE_TYPE_PURE,           // 纯粹伤害不会被任何护甲或魔抗减少
}

// 伤害标记
public enum AbilityDamageFlag {
    DAMAGE_FLAG_NONE = 1,      // 无
    DAMAGE_FLAG_CRIT = 2,      // 暴击
    DAMAGE_FLAG_LIFELINK = 4,  // 吸血
}

// 技能消耗类型
public enum AbilityCostType {
    NO_COST,
    ENERGY
}

// 技能AI的目标选取条件
public enum AbilityUnitAITargetCondition {
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
public enum AbilityBranch {
    ABILITY_BRANCH_PHYSICAL = 1,//物理类型的技能，可被缴械等物理沉默导致技能不可释放
    ABILITY_BRANCH_MAGICAL, //法术类型的技能，可被沉默等法术沉默导致技能不可释放
}

// 治疗
public enum AbilityHealFlag {
    HEAL_FLAG_NONE = 1,     
    HEAL_FLAG_CRIT = 2,     // 治疗暴击
}

/// <summary>
/// 技能行为
/// "AbilityBehavior" (技能行为) 的全部类型，会使技能有不用的释放方式。
/// 你能够使用|来分割不同的常量，比如说 "DOTA_ABILITY_BEHAVIOR_HIDDEN | DOTA_ABILITY_BEHAVIOR_NO_TARGET" - 这些空格是很重要的！
/// </summary>
[Flags]
public enum AbilityBehavior {
    DOTA_ABILITY_BEHAVIOR_HIDDEN               = 1 << 0, //这个技能是单位所拥有的技能，但是不会在HUD上显示。
    DOTA_ABILITY_BEHAVIOR_PASSIVE              = 1 << 1, //这个技能是一个被动技能，不能被使用，但是会在HUD上显示。
    DOTA_ABILITY_BEHAVIOR_NO_TARGET            = 1 << 2, // 无目标技能，不需要选择目标释放，按下技能按钮即可释放
    DOTA_ABILITY_BEHAVIOR_UNIT_TARGET          = 1 << 3, // 单位目标技能，需要目标释放，需要 AbilityUnitTargetTeam 和 AbilityUnitTargetType ，参见
    DOTA_ABILITY_BEHAVIOR_POINT                = 1 << 4, // 点目标技能，可以对鼠标指向的任意位置释放，如果点击单位也只是对其位置释放
    DOTA_ABILITY_BEHAVIOR_AOE                  = 1 << 5, //这个技能将会显示技能释放的范围，有点像DOTA_ABILITY_BEHAVIOR_POINT类的技能，但是会显示一个范围。
    DOTA_ABILITY_BEHAVIOR_NOT_LEARNABLE        = 1 << 6, //这个技能将能被释放，或者拥有对应的释放机制，但是不能被学习。（一般是用在类似炸弹人的引爆炸弹技能）。
    DOTA_ABILITY_BEHAVIOR_CHANNELLED           = 1 << 7, //持续性施法技能，如果施法者移动或者被沉默，这个技能将会被中断。
    DOTA_ABILITY_BEHAVIOR_ITEM                 = 1 << 8, //这个技能绑定了一个物品。
    DOTA_ABILITY_BEHAVIOR_TOGGLE               = 1 << 9, //切换类技能。
    DOTA_ABILITY_BEHAVIOR_DIRECTIONAL          = 1 << 10, // 方向技能，拥有一个从英雄出发的方向，例如白虎的箭和屠夫的钩子 Has a direction from the hero, such as miranas arrow or pudge's hook.
    DOTA_ABILITY_BEHAVIOR_IMMEDIATE            = 1 << 11, //这个技能将会被立即释放，不会进入操作序列。
    DOTA_ABILITY_BEHAVIOR_AUTOCAST             = 1 << 12, //这个技能可以被自动释放。
    DOTA_ABILITY_BEHAVIOR_NOASSIST             = 1 << 13, //这个技能将不会有辅助网格。
    DOTA_ABILITY_BEHAVIOR_AURA                 = 1 << 14, //这个技能是一个光环技能，Not really used other than to tag the ability as such.
    DOTA_ABILITY_BEHAVIOR_ATTACK               = 1 << 15, //这个技能是一个法球技能，不能对魔法免疫目标生效，
    DOTA_ABILITY_BEHAVIOR_DONT_RESUME_MOVEMENT = 1 << 16, //这个技能在释放完成之后不会继续之前的移动操作，只能和无目标或者立即释放类技能配合使用。
    DOTA_ABILITY_BEHAVIOR_ROOT_DISABLES        = 1 << 17, //这个技能在单位被定身的时候无法使用。
    DOTA_ABILITY_BEHAVIOR_UNRESTRICTED         = 1 << 18, //这个技能在释放指令被限制的时候也能被使用。
    DOTA_ABILITY_BEHAVIOR_IGNORE_PSEUDO_QUEUE  = 1 << 19, //这个技能在被眩晕，施法和被强制攻击的时候也能使用，只能和自动释放类DOTA_ABILITY_BEHAVIOR_AUTOCAST配合使用。
    DOTA_ABILITY_BEHAVIOR_IGNORE_CHANNEL       = 1 << 20, //这个技能即使施法被中断也能继续释放。
    DOTA_ABILITY_BEHAVIOR_DONT_CANCEL_MOVEMENT = 1 << 21, //Doesn't cause certain modifiers to end, 目前未知，只在信使的速度爆发有见到。
    DOTA_ABILITY_BEHAVIOR_DONT_ALERT_TARGET    = 1 << 22, //这个技能在指定敌人释放的时候将不会惊醒他们。
    DOTA_ABILITY_BEHAVIOR_DONT_RESUME_ATTACK   = 1 << 23, //这个技能在释放完成之后，将不会恢复对之前目标的自动攻击，只能配合无目标，非立即释放类和指定单位目标类技能使用。
    DOTA_ABILITY_BEHAVIOR_NORMAL_WHEN_STOLEN   = 1 << 24, //这个技能在被偷取之后，依然使用之前的施法前摇。
    DOTA_ABILITY_BEHAVIOR_IGNORE_BACKSWING     = 1 << 25, //这个技能将会无视施法后摇。
    DOTA_ABILITY_BEHAVIOR_RUNE_TARGET          = 1 << 26, //这个技能能以神符为目标。
    ABILITY_BEHAVIOR_RADIUS_AOE, //圆形AOE技能，需要配合AbilityAoeRadius来使用 项目定制，todo删除
    ABILITY_BEHAVIOR_SECTOR_AOE,//扇形AOE技能，需要配合AbilityAoeSector来使用 项目定制，todo删除
}

/// <summary>
/// 技能数值来源
/// </summary>
public enum AbilityValueSourceType {
    SOURCE_TYPE_PHYSICAL_ATTACK,
    SOURCE_TYPE_MAGICAL_ATTACK,
    SOURCE_TYPE_CASTER_MAX_HP,
    SOURCE_TYPE_CASTER_CURRENT_HP,
    SOURCE_TYPE_CASTER_LOST_HP,
    SOURCE_TYPE_TARGET_MAX_HP,
    SOURCE_TYPE_TARGET_CURRENT_HP,
    SOURCE_TYPE_TARGET_LOST_HP,
}

#region 操作目标 Action Target Values

// "Target" is one bitch of a key.
// https://moddota.com/abilities/datadriven/all-about-the-target

/// <summary>
/// 单个目标
/// </summary>
public enum ActionSingleTarget {
    CASTER,// 施法者
    TARGET,// 目标
    POINT,// 点
    ATTACKER,// 攻击者
    UNIT,// 单位
}

#region 多个目标
/*
小范例：
"Target"{
    "Center"    "POINT"
    "Teams"     "DOTA_UNIT_TARGET_TEAM_ENEMY"
    "Radius"    "300"
}
想要在某个区域里选中多个目标，就需要在 Target代码块 中填入下面的参数：
Center - 以目标为搜索范围的中心
Radius[整数值] - 搜索目标的范围
Teams - 根据队伍筛选目标（一样可以用 | 来指定多种类型）
Types - 指定的类型，ExcludeTypes - 排除对应的类型 （一样可以用 | 来指定多种类型）
Flags - 指定目标的状态，ExcludeFlags - 排除对应状态的目标 （一样可以用 | 来指定多种类型）
MaxTargets[整数值] - 最多目标数量
Random[布尔值] - 在指定了最多目标数量之后，是否额外随机多选择一个单位。
ScriptSelectPoints - *暂无说明*
ScriptFile, Function, Radius, Count
注意: 	将Random 和 MaxTargets 共同使用，将Random 设置为0将会使操作只影响目标区域内MaxTargets或者更少的单位。
 */

/// <summary>
/// 技能目标中心点（AOE区域以中心点展开）
/// </summary>
public enum ActionMultipleTargets
{
    CASTER,// 施法者
    TARGET,// 目标
    POINT,// 点
    ATTACKER,// 攻击者
    UNIT,// 单位
    PROJECTILE,// 抛射物
}

/// <summary>
///  根据队伍筛选目标（一样可以用 | 来指定多种类型）
/// </summary>
public enum AbilityUnitTargetTeam
{
    DOTA_UNIT_TARGET_TEAM_NONE,// 无
    DOTA_UNIT_TARGET_TEAM_BOTH,// 双方队伍
    DOTA_UNIT_TARGET_TEAM_CUSTOM,// 普通队伍
    DOTA_UNIT_TARGET_TEAM_ENEMY,// 敌方队伍
    DOTA_UNIT_TARGET_TEAM_FRIENDLY,// 友方队伍
}

/// <summary>
/// 类型
/// </summary>
public enum AbilityUnitTargetType
{
    DOTA_UNIT_TARGET_NONE,// 没有
    DOTA_UNIT_TARGET_ALL,// 任意，包括隐藏的实体
    DOTA_UNIT_TARGET_BASIC,// 基本单位, 包括召唤单位
    DOTA_UNIT_TARGET_BUILDING,// 塔和建筑 npc_dota_tower, npc_dota_building  DOTA_NPC_UNIT_RELATIONSHIP_TYPE_BUILDING
    DOTA_UNIT_TARGET_COURIER,// 信使和飞行信使 npc_dota_courier, npc_dota_flying_courier  DOTA_NPC_UNIT_RELATIONSHIP_TYPE_COURIER
    DOTA_UNIT_TARGET_CREEP,// 野怪 npc_dota_creature, npc_dota_creep 与BASIC类似但是可能不包括一些召唤单位 例子: 骨弓死亡契约
    DOTA_UNIT_TARGET_CUSTOM,// 未开放? 例子: 水人复制, TB灵魂隔断, 谜团恶魔转化, 艾欧链接, 小狗感染...
    DOTA_UNIT_TARGET_HERO,// 英雄
    DOTA_UNIT_TARGET_MECHANICAL,// 机械单位（投石车） npc_dota_creep_siege  DOTA_NPC_UNIT_RELATIONSHIP_TYPE_SIEGE
    DOTA_UNIT_TARGET_TREE,// 树木,ent_dota_tree 树 例子: 吃树, 补刀斧
    DOTA_UNIT_TARGET_OTHER,// 任何前面不包括的单位
}

/// <summary>
/// 标签允许对默认被忽略的目标单位 (例如魔法免疫敌人)施法, 或者忽略特定的单位类型 (例如远古单位和魔免友军)来允许对其施法.
/// </summary>
public enum AbilityUnitTargetFlag
{
    DOTA_UNIT_TARGET_FLAG_CHECK_DISABLE_HELP,//禁用帮助的单位 尚不确定数据驱动技能如何使用？
    DOTA_UNIT_TARGET_FLAG_DEAD,//死亡单位忽略
    DOTA_UNIT_TARGET_FLAG_FOW_VISIBLE,//单位离开视野时打断 例子: 魔法汲取, 生命汲取
    DOTA_UNIT_TARGET_FLAG_INVULNERABLE,//拥有MODIFIER_STATE_INVULNERABLE的单位（无敌单位） 例子: 暗杀, 召回, 巨力重击...
    DOTA_UNIT_TARGET_FLAG_MAGIC_IMMUNE_ENEMIES,//指向拥有MODIFIER_STATE_MAGIC_IMMUNE （魔法免疫）的敌人单位 例子: 根须缠绕, 淘汰之刃, 原始咆哮...
    DOTA_UNIT_TARGET_FLAG_MANA_ONLY,//在npc_unit中拥有魔法没有"StatusMana" "0"的单位
    DOTA_UNIT_TARGET_FLAG_MELEE_ONLY,//拥有攻击类型DOTA_UNIT_CAP_MELEE_ATTACK的单位
    DOTA_UNIT_TARGET_FLAG_NO_INVIS,//忽略拥有MODIFIER_STATE_INVISIBLE的不可见单位
    DOTA_UNIT_TARGET_FLAG_NONE,//缺省默认值
    DOTA_UNIT_TARGET_FLAG_NOT_ANCIENTS,//忽略单位，使用标签"IsAncient" "1" 例子: 麦达斯之手
    DOTA_UNIT_TARGET_FLAG_NOT_ATTACK_IMMUNE,//拥有MODIFIER_STATE_ATTACK_IMMUNE的单位（攻击免疫单位）
    DOTA_UNIT_TARGET_FLAG_NOT_CREEP_HERO,//忽略单位，使用标签"ConsideredHero" "1" 例子: 星体禁锢, 崩裂禁锢, 灵魂隔断
    DOTA_UNIT_TARGET_FLAG_NOT_DOMINATED,//拥有MODIFIER_STATE_DOMINATED的单位（被支配的单位）
    DOTA_UNIT_TARGET_FLAG_NOT_ILLUSIONS,//拥有MODIFIER_PROPERTY_IS_ILLUSION的单位（幻象单位）
    DOTA_UNIT_TARGET_FLAG_NOT_MAGIC_IMMUNE_ALLIES,//忽略拥有MODIFIER_STATE_MAGIC_IMMUNE（魔法免疫）的友军 例子: 痛苦之源噩梦
    DOTA_UNIT_TARGET_FLAG_NOT_NIGHTMARED,//拥有MODIFIER_STATE_NIGHTMARED的单位（噩梦中单位）
    DOTA_UNIT_TARGET_FLAG_NOT_SUMMONED,//通过SpawnUnit Action创建的单位
    DOTA_UNIT_TARGET_FLAG_OUT_OF_WORLD,//拥有MODIFIER_STATE_OUT_OF_GAME的单位（离开游戏的单位）
    DOTA_UNIT_TARGET_FLAG_PLAYER_CONTROLLED,//玩家控制的单位，接近Lua IsControllableByAnyPlayer()
    DOTA_UNIT_TARGET_FLAG_RANGED_ONLY,//拥有攻击类型DOTA_UNIT_CAP_RANGED_ATTACK的单位
}

#endregion

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

/*
是只有data-driven类技能才能使用的，这个区域定义了一系列的Modifiers（一般可以认为是Buff或者DeBuff），
每个Modifier通过在其中定义的KV(KeyValues)来定义。 参数有以下：
Properties	属性（例如：忽视无敌，可叠加，无，正常 ）
Duration	持续时间	浮点值
EffectAttachType	特效附着点类型	原点，头顶，胸膛，头部，自定义原点，世界原点
EffectName	特效名称	字符串(路径+特效名)
IsBuff	是否增益魔法	布尔值（1为增益魔法）
IsDebuff	是否减益魔法	布尔值（1为减益魔法）
IsHidden	是否隐藏	布尔值（1为隐藏）
IsPurgable	能否被驱散	布尔值（1为可被驱散）
OverrideAnimation	覆盖模型动作	攻击动作，技能释放动作(1~6)，受伤动作，出生动作，回程动作，胜利动作ACT_DOTA_ATTACK, ACT_DOTA_CAST_ABILITY_1 (2, 3, 4, 5, 6), ACT_DOTA_DISABLED, ACT_DOTA_SPAWN, ACT_DOTA_TELEPORT, ACT_DOTA_VICTORY
Passive	被动	布尔值（1为被动技能，会自动添加该Modifiers）
TextureName	图标名称	字符串
ThinkInterval	Think间隔	浮点值
 */

public enum ModifierProperty {
    MODIFIER_PROPERTY_ABSOLUTE_NO_DAMAGE_MAGICAL,// 所有魔法攻击无效
    MODIFIER_PROPERTY_ABSOLUTE_NO_DAMAGE_PHYSICAL,// 所有物理攻击无效
    // MODIFIER_PROPERTY_ABSOLUTE_NO_DAMAGE_PURE	所有神圣伤害无效
    // MODIFIER_PROPERTY_ABSORB_SPELL	偷取法术？
    // MODIFIER_PROPERTY_ATTACK_RANGE_BONUS	修改攻击范围
    // MODIFIER_PROPERTY_ATTACK_RANGE_BONUS_UNIQUE	攻击距离增益（不叠加）
    // MODIFIER_PROPERTY_ATTACKSPEED_BONUS_CONSTANT	修改攻击速度
    // MODIFIER_PROPERTY_ATTACKSPEED_BONUS_CONSTANT_POWER_TREADS	*暂无说明*
    // MODIFIER_PROPERTY_ATTACKSPEED_BONUS_CONSTANT_SECONDARY	*暂无说明*
    // MODIFIER_PROPERTY_AVOID_CONSTANT	虚空假面的闪避？
    // MODIFIER_PROPERTY_AVOID_SPELL	虚空假面的法术闪避？
    // MODIFIER_PROPERTY_BASEATTACK_BONUSDAMAGE	修改基础攻击力
    // MODIFIER_PROPERTY_BASE_ATTACK_TIME_CONSTANT	设定基础攻击间隔
    // MODIFIER_PROPERTY_BASEDAMAGEOUTGOING_PERCENTAGE	修改基础攻击伤害
    // MODIFIER_PROPERTY_BASE_MANA_REGEN	修改基础魔法回复数值，对百分比回魔有影响
    // MODIFIER_PROPERTY_BONUS_DAY_VISION	修改白天的视野距离
    // MODIFIER_PROPERTY_BONUS_NIGHT_VISION	修改夜间的视野距离
    // MODIFIER_PROPERTY_BONUS_VISION_PERCENTAGE	按百分比修改视野距离
    // MODIFIER_PROPERTY_CAST_RANGE_BONUS	施法距离增益
    // MODIFIER_PROPERTY_CHANGE_ABILITY_VALUE	改变技能数值
    // MODIFIER_PROPERTY_COOLDOWN_PERCENTAGE_STACKING	冷却时间百分比堆叠
    // MODIFIER_PROPERTY_COOLDOWN_REDUCTION_CONSTANT	减少冷却时间
    // MODIFIER_PROPERTY_DAMAGEOUTGOING_PERCENTAGE	按百分比修改攻击力，负数降低攻击，正数提高攻击
    // MODIFIER_PROPERTY_DAMAGEOUTGOING_PERCENTAGE_ILLUSION	降低幻象攻击力比例
    // MODIFIER_PROPERTY_DEATHGOLDCOST	修改死亡损失的金钱
    // MODIFIER_PROPERTY_DISABLE_AUTOATTACK	禁止自动攻击
    // MODIFIER_PROPERTY_DISABLE_HEALING	禁止生命回复(1为禁止)
    // MODIFIER_PROPERTY_DISABLE_TURNING	禁止转身
    // MODIFIER_PROPERTY_EVASION_CONSTANT	闪避
    // MODIFIER_PROPERTY_EXTRA_HEALTH_BONUS	*无效*额外生命值加成
    // MODIFIER_PROPERTY_EXTRA_MANA_BONUS	*无效*额外魔法值加成
    // MODIFIER_PROPERTY_EXTRA_STRENGTH_BONUS	*无效*额外力量加成
    // MODIFIER_PROPERTY_FORCE_DRAW_MINIMAP	*暂无说明*
    // MODIFIER_PROPERTY_HEALTH_BONUS	修改目前血量
    // MODIFIER_PROPERTY_HEALTH_REGEN_CONSTANT	固定的生命回复数值
    // MODIFIER_PROPERTY_HEALTH_REGEN_PERCENTAGE	根据装备带来的最大血量所产生的血量回复数值
    // MODIFIER_PROPERTY_IGNORE_CAST_ANGLE	忽略施法角度
    // MODIFIER_PROPERTY_INCOMING_DAMAGE_PERCENTAGE	按百分比修改受到的所有伤害，负数降低伤害，正数加深伤害
    // MODIFIER_PROPERTY_INCOMING_PHYSICAL_DAMAGE_CONSTANT	所受物理伤害数值（数值物理伤害减免/增加）
    // MODIFIER_PROPERTY_INCOMING_PHYSICAL_DAMAGE_PERCENTAGE	按百分比修改受到的物理伤害，负数降低伤害，正数加深伤害
    // MODIFIER_PROPERTY_INCOMING_SPELL_DAMAGE_CONSTANT	按百分比修改受到的技能伤害，负数降低伤害，正数加深伤害
    // MODIFIER_PROPERTY_INVISIBILITY_LEVEL	隐身等级？
    // MODIFIER_PROPERTY_IS_ILLUSION	是否为某个单位的幻象
    // MODIFIER_PROPERTY_IS_SCEPTER	是否携带蓝杖？
    // MODIFIER_PROPERTY_LIFETIME_FRACTION	*暂无说明*
    // MODIFIER_PROPERTY_MAGICAL_RESISTANCE_BONUS	魔法抗性，对神圣伤害无效，可以累加
    // MODIFIER_PROPERTY_MAGICAL_RESISTANCE_DECREPIFY_UNIQUE	骨法的衰老，影响魔法抗性，不可累加
    // MODIFIER_PROPERTY_MAGICAL_RESISTANCE_ITEM_UNIQUE	魔法抗性，对神圣伤害无效，不可以累加
    // MODIFIER_PROPERTY_MAGICDAMAGEOUTGOING_PERCENTAGE	魔法输出百分比（百分比法伤增益/减益）
    // MODIFIER_PROPERTY_MANA_BONUS	修改目前魔法量
    // MODIFIER_PROPERTY_MANA_REGEN_CONSTANT	修改基础魔法回复数值，对百分比回魔没有影响
    // MODIFIER_PROPERTY_MANA_REGEN_CONSTANT_UNIQUE	修改基础魔法回复数值，对百分比回魔没有影响，且 不可累积
    // MODIFIER_PROPERTY_MANA_REGEN_PERCENTAGE	修改基础魔法回复数值
    // MODIFIER_PROPERTY_MANA_REGEN_TOTAL_PERCENTAGE	修改所有魔法回复数值
    // MODIFIER_PROPERTY_MAX_ATTACK_RANGE	最大攻击距离增益
    // MODIFIER_PROPERTY_MIN_HEALTH	血量在设定值以下是不能杀死（斧王的斩杀依然有效）
    // MODIFIER_PROPERTY_MISS_PERCENTAGE	增加miss的几率
    // MODIFIER_PROPERTY_MODEL_CHANGE	设定模型
    // MODIFIER_PROPERTY_MODEL_SCALE	设定模型大小
    // MODIFIER_PROPERTY_MOVESPEED_ABSOLUTE	设置移动速度
    // MODIFIER_PROPERTY_MOVESPEED_BASE_OVERRIDE	设定基础移动速度
    // MODIFIER_PROPERTY_MOVESPEED_BONUS_CONSTANT	增加移动速度数值
    // MODIFIER_PROPERTY_MOVESPEED_BONUS_PERCENTAGE	百分比增加移动速度，自身不叠加
    // MODIFIER_PROPERTY_MOVESPEED_BONUS_PERCENTAGE_UNIQUE	独立百分比增加移动速度，不叠加
    // MODIFIER_PROPERTY_MOVESPEED_BONUS_UNIQUE	增加移动速度数值，不叠加，物品版本
    // MODIFIER_PROPERTY_MOVESPEED_LIMIT	限制移动速度
    // MODIFIER_PROPERTY_MOVESPEED_MAX	设置最大移动速度
    // MODIFIER_PROPERTY_NEGATIVE_EVASION_CONSTANT	降低闪避概率
    // MODIFIER_PROPERTY_OVERRIDE_ANIMATION	强制播放模型动作
    // MODIFIER_PROPERTY_OVERRIDE_ANIMATION_RATE	设置播放模型动作快慢
    // MODIFIER_PROPERTY_OVERRIDE_ANIMATION_WEIGHT	强制播放模型动作_重？
    // MODIFIER_PROPERTY_OVERRIDE_ATTACK_MAGICAL	魔法攻击
    // MODIFIER_PROPERTY_PERSISTENT_INVISIBILITY	永久性隐身
    // MODIFIER_PROPERTY_PHYSICAL_ARMOR_BONUS	增加护甲
    // MODIFIER_PROPERTY_PHYSICAL_ARMOR_BONUS_ILLUSIONS	增加幻象的护甲
    // MODIFIER_PROPERTY_PHYSICAL_ARMOR_BONUS_UNIQUE	增加护甲，不可叠加
    // MODIFIER_PROPERTY_PHYSICAL_ARMOR_BONUS_UNIQUE_ACTIVE	改变圆盾减伤的效果？
    // MODIFIER_PROPERTY_PHYSICAL_CONSTANT_BLOCK	数值减免伤害？
    // MODIFIER_PROPERTY_POST_ATTACK	增加攻击力？
    // MODIFIER_PROPERTY_PREATTACK_BONUS_DAMAGE	修改附加攻击力
    // MODIFIER_PROPERTY_PREATTACK_BONUS_DAMAGE_POST_CRIT	以增加伤害的方式修改伤害值，不计入暴击计算
    // MODIFIER_PROPERTY_PREATTACK_CRITICALSTRIKE	致命一击
    // MODIFIER_PROPERTY_PROCATTACK_BONUS_DAMAGE_COMPOSITE	修改在普通攻击后计算的神圣伤害
    // MODIFIER_PROPERTY_PROCATTACK_BONUS_DAMAGE_MAGICAL	修改在普通攻击后计算的魔法伤害
    // MODIFIER_PROPERTY_PROCATTACK_BONUS_DAMAGE_PHYSICAL	修改在普通攻击后计算的物理伤害
    // MODIFIER_PROPERTY_PROCATTACK_BONUS_DAMAGE_PURE	修改在普通攻击后计算的神圣伤害
    // MODIFIER_PROPERTY_PROCATTACK_FEEDBACK	法力燃烧？
    // MODIFIER_PROPERTY_PROVIDES_FOW_POSITION	*暂无说明*
    // MODIFIER_PROPERTY_REINCARNATION	不朽之守护或者是骷髅王的大招？
    // MODIFIER_PROPERTY_RESPAWNTIME	修改重生时间
    // MODIFIER_PROPERTY_RESPAWNTIME_PERCENTAGE	百分比修改重生时间
    // MODIFIER_PROPERTY_RESPAWNTIME_STACKING	累积重生时间
    // MODIFIER_PROPERTY_STATS_AGILITY_BONUS	修改敏捷
    // MODIFIER_PROPERTY_STATS_INTELLECT_BONUS	修改智力
    // MODIFIER_PROPERTY_STATS_STRENGTH_BONUS	修改力量
    // MODIFIER_PROPERTY_SUPER_ILLUSION_WITH_ULTIMATE	VS A杖大招的那个幻象
    // MODIFIER_PROPERTY_TOOLTIP	可被用于任何提示， 比如臂章的血量移除
    // MODIFIER_PROPERTY_TOTAL_CONSTANT_BLOCK	减免所有来源的伤害
    // MODIFIER_PROPERTY_TOTAL_CONSTANT_BLOCK_UNAVOIDABLE_PRE_ARMOR	对于自动攻击的伤害减免
    // MODIFIER_PROPERTY_TOTALDAMAGEOUTGOING_PERCENTAGE	失效，不工作
    // MODIFIER_PROPERTY_TRANSLATE_ACTIVITY_MODIFIERS	动作修改？
    // MODIFIER_PROPERTY_TRANSLATE_ATTACK_SOUND	攻击音效修改？
    // MODIFIER_PROPERTY_TURN_RATE_PERCENTAGE	百分比修改转向速度
    // 项目自定义
    // MODIFIER_PROPERTY_PHYSICAL_ATTACK = 1,                               // 修改物理攻击
    // MODIFIER_PROPERTY_PHYSICAL_DEFENCE = 2,                              // 修改物理防御
    // MODIFIER_PROPERTY_PHYSICAL_PENETRATION = 3,                          // 修改物理穿透
    // MODIFIER_PROPERTY_MAGIC_ATTACK = 4,                                  // 修改魔法攻击
    // MODIFIER_PROPERTY_MAGIC_DEFENCE = 5,                                 // 修改魔法防御
    // MODIFIER_PROPERTY_MAGIC_PENETRATION = 6,                             // 修改魔法穿透
    //
    // MODIFIER_PROPERTY_CRIT = 7,                                          // 修改暴击
    // MODIFIER_PROPERTY_DODGE = 8,                                         // 修改闪避数值
    // MODIFIER_PROPERTY_HIT_RATE = 9,                                      // 修改命中率
    // MODIFIER_PROPERTY_LIFELINK_LEVEL = 10,                               // 修改吸血等级
    // MODIFIER_PROPERTY_MAX_HP = 11,                                       // 修改最大HP
    //
    // MODIFIER_PROPERTY_STATS_STRENGTH_BONUS = 12,                         // 修改力量
    // MODIFIER_PROPERTY_STATS_AGILITY_BONUS = 13,                          // 修改敏捷
    // MODIFIER_PROPERTY_STATS_INTELLECT_BONUS = 14,                        // 修改智力
    //
    // MODIFIER_PROPERTY_INCOMING_TAKE_HEAL_VALUE_PERCENTAGE = 15,          // 修改受到的治疗效果万分比，负数降低治疗效果，正数增加治疗效果
    // MODIFIER_PROPERTY_INCOMING_DEAL_HEAL_VALUE_PERCENTAGE = 16,          // 修改造成的治疗效果万分比，负数降低治疗效果，正数增加治疗效果
    //
    // MODIFIER_PROPERTY_INCOMING_TAKE_PHYSICAL_DAMAGE_PERCENTAGE = 17,     // 修改受到的物理伤害万分比，负数降低伤害，正数增加伤害
    // MODIFIER_PROPERTY_INCOMING_DEAL_PHYSICAL_DAMAGE_PERCENTAGE = 18,     // 修改造成的物理伤害万分比，负数降低伤害，正数增加伤害
    // MODIFIER_PROPERTY_INCOMING_TAKE_MAGIC_DAMAGE_PERCENTAGE = 19,        // 修改受到的魔法伤害万分比，负数降低伤害，正数增加伤害
    // MODIFIER_PROPERTY_INCOMING_DEAL_MAGIC_DAMAGE_PERCENTAGE = 20,        // 修改造成的魔法伤害万分比，负数降低伤害，正数增加伤害
    // MODIFIER_PROPERTY_INCOMING_TAKE_ALL_DAMAGE_PERCENTAGE = 21,          // 修改受到的所有伤害万分比，负数降低伤害，正数增加伤害
    // MODIFIER_PROPERTY_INCOMING_DEAL_ALL_DAMAGE_PERCENTAGE = 22,          // 修改造成的所有伤害万分比，负数降低伤害，正数增加伤害
    //
    // MODIFIER_PROPERTY_PHYSICAL_SHIELD = 23,                              // 物理护盾
    // MODIFIER_PROPERTY_MAGIC_SHIELD = 24,                                 // 魔法护盾
    // MODIFIER_PROPERTY_SHIELD = 25,                                       // 所有伤害都可以吸收的护盾
}

/// <summary>
/// 特效要附着的节点
/// </summary>
public enum ModifierEffectAttachType {
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
/// modifier 要修改的状态
/// Modifier状态和Modifier属性十分类似，只不过它只有三个选项：
/// "MODIFIER_STATE_VALUE_NO_ACTION"(不作为),
/// "MODIFIER_STATE_VALUE_ENABLED"(启用)
/// "MODIFIER_STATE_VALUE_DISABLED"(禁用)。
/// 以下的modifier示例给单位添加了一个简单的眩晕粒子特效，覆盖了单位的模型操作，让单位进入了眩晕状态。
/// </summary>
/*
"creature_bash_ministun"{
    "Duration"              "%duration"
    "EffectName"            "generic_stunned"
    "EffectAttachType"      "follow_overhead"
    "Duration"              "%stun_duration"
    "OverrideAnimation"     "ACT_DOTA_DISABLED"
    "States"{
        "MODIFIER_STATE_STUNNED" "MODIFIER_STATE_VALUE_ENABLED"
    }
}
*/
public enum ModifierStates {
    MODIFIER_STATE_ATTACK_IMMUNE,// 攻击免疫状态
    MODIFIER_STATE_BLIND,// 致盲状态？无法物理攻击？
    // MODIFIER_STATE_BLOCK_DISABLED	禁用伤害减免？
    // MODIFIER_STATE_CANNOT_MISS	不能闪避？
    // MODIFIER_STATE_COMMAND_RESTRICTED	禁魔状态
    // MODIFIER_STATE_DISARMED	缴械状态
    // MODIFIER_STATE_DOMINATED	支配状态？
    // MODIFIER_STATE_EVADE_DISABLED	禁用躲避？
    // MODIFIER_STATE_FLYING	飞行状态
    // MODIFIER_STATE_FROZEN	冷冻状态
    // MODIFIER_STATE_HEXED	妖术状态
    // MODIFIER_STATE_INVISIBLE	隐身状态
    // MODIFIER_STATE_INVULNERABLE	无敌状态
    // MODIFIER_STATE_LOW_ATTACK_PRIORITY	低的攻击优先级
    // MODIFIER_STATE_MAGIC_IMMUNE	魔法免疫状态
    // MODIFIER_STATE_MUTED	禁用物品状态
    // MODIFIER_STATE_NIGHTMARED	催眠状态
    // MODIFIER_STATE_NO_HEALTH_BAR	没有生命条
    // MODIFIER_STATE_NO_TEAM_MOVE_TO	没有移动到队伍状态
    // MODIFIER_STATE_NO_TEAM_SELECT	没有选择队伍状态
    // MODIFIER_STATE_NOT_ON_MINIMAP	不在小地图状态
    // MODIFIER_STATE_NOT_ON_MINIMAP_FOR_ENEMIES	敌人不在小地图状态
    // MODIFIER_STATE_NO_UNIT_COLLISION	没有单位碰撞状态
    // MODIFIER_STATE_OUT_OF_GAME	离开游戏状态
    // MODIFIER_STATE_PASSIVES_DISABLED	禁用被动技能状态
    // MODIFIER_STATE_PROVIDES_VISION	提供视野状态
    // MODIFIER_STATE_ROOTED	被缠绕状态
    // MODIFIER_STATE_SILENCED	沉默状态
    // MODIFIER_STATE_SOFT_DISARMED	软解除武装状态
    // MODIFIER_STATE_SPECIALLY_DENIABLE	*暂无说明*
    // MODIFIER_STATE_STUNNED	眩晕状态
    // MODIFIER_STATE_UNSELECTABLE	无法选取状态
    // 项目自定义
    // MODIFIER_STATE_STUNNED = 1,                     // 眩晕
    // MODIFIER_STATE_FROZEN = 2,                      // 冰冻
    // MODIFIER_STATE_NIGHTMARED = 3,                  // 睡眠
    // MODIFIER_STATE_SILENCED = 4,                    // 沉默（不能释放魔法技能）
    // MODIFIER_STATE_DISARMED = 5,                    // 缴械（不能释放物理技能）
    // MODIFIER_STATE_ROOTED = 6,                      // 缠绕状态（不可移动，不能释放物理技能）
    // MODIFIER_STATE_TAUNTED = 7,                     // 嘲讽状态（强制攻击目标）
    // MODIFIER_STATE_BANISH = 8,                      // 放逐
    // MODIFIER_STATE_CHARM = 9,                       // 魅惑
    // MODIFIER_STATE_ENERGY_RECOVERY_DISABLED = 10,   // 禁止能量回复
    // MODIFIER_STATE_HIT = 11,                        // 受击
    // MODIFIER_STATE_CONTROLLER_IMMUNE = 12,          // 免疫控制
}


/// <summary>
/// modifier 事件
/// Modifiers也可以定义一些触发事件，以下的这些事件能够在modifier中定义
/// </summary>
public enum ModifierEvents {
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

/*
Modifier事件列表
这些事件并不可以应用在所有的数据驱动类技能中，仅作为参考。
MODIFIER_EVENT_ON_ABILITY_END_CHANNEL	当持续性施法完成
MODIFIER_EVENT_ON_ABILITY_EXECUTED	当施法执行完
MODIFIER_EVENT_ON_ABILITY_START	当施法开始时
MODIFIER_EVENT_ON_ATTACK	当攻击时
MODIFIER_EVENT_ON_ATTACK_ALLIED	当攻击盟军时
MODIFIER_EVENT_ON_ATTACKED	当攻击结束时
MODIFIER_EVENT_ON_ATTACK_FAIL	当攻击失败
MODIFIER_EVENT_ON_ATTACK_LANDED	当攻击击中时
MODIFIER_EVENT_ON_ATTACK_START	当攻击开始时
MODIFIER_EVENT_ON_BREAK_INVISIBILITY	当打破隐身状态
MODIFIER_EVENT_ON_DEATH	当死亡时
MODIFIER_EVENT_ON_HEAL_RECEIVED	当收到治疗
MODIFIER_EVENT_ON_HEALTH_GAINED	当获得生命值
MODIFIER_EVENT_ON_HERO_KILLED	当英雄死亡
MODIFIER_EVENT_ON_MANA_GAINED	当获得魔法值
MODIFIER_EVENT_ON_ORB_EFFECT	当在法球效果
MODIFIER_EVENT_ON_ORDER	当命令结束时
MODIFIER_EVENT_ON_PROCESS_UPGRADE	当在升级过程中
MODIFIER_EVENT_ON_PROJECTILE_DODGE	当闪避弹道时
MODIFIER_EVENT_ON_REFRESH	当刷新时
MODIFIER_EVENT_ON_RESPAWN	当重生时
MODIFIER_EVENT_ON_SPENT_MANA	当花费魔法值时
MODIFIER_EVENT_ON_STATE_CHANGED	当状态改变时
MODIFIER_EVENT_ON_TAKEDAMAGE	当带来伤害时
MODIFIER_EVENT_ON_TAKEDAMAGE_REAPERSCYTHE	当在死神镰刀下带来伤害时
MODIFIER_EVENT_ON_TELEPORTED	当在传送结束时
MODIFIER_EVENT_ON_TELEPORTING	当正在传送时
MODIFIER_EVENT_ON_UNIT_MOVED	当单位移动时
 */
#endregion

#region Client

/// <summary>
/// 技能指示器类型
/// </summary>
public enum AbilityIndicatorType {
    CIRCLE_AREA,
    LINE_AREA,
    SECTOR60_AREA,
    SECTOR90_AREA,
    SECTOR120_AREA,
    RANGE_AREA,
    SEGMENT,
    SEGMENT_AREA
}

public enum ProjectileType {
    Linear,      // 线性
    Tracking,    // 追踪
    Bouncing,    // 弹跳
}

#endregion