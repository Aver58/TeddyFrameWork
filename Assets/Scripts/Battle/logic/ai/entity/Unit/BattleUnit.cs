#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    GuardUnit.cs
 Author:      Zeng Zhiwei
 Time:        2020/3/5 13:43:29
=====================================================
*/
#endregion

using System.Collections.Generic;
using Aver3;
using UnityEngine;

// 没有行为树
public class BattleUnit : Unit
{
    public BattleCamp camp { get; }
    public BattleCamp enemyCamp { get;set; }
    public int hash { get { return GetHashCode(); } }
    public BattleUnit target { get; set; }
    public List<Ability> abilities;

    private static Dictionary<string, AbilityCastType> m_stringToCastTypeMap = new Dictionary<string, AbilityCastType> {
        { "attack",AbilityCastType.ATTACK},
        { "skill1",AbilityCastType.SKILL1},
        { "skill2",AbilityCastType.SKILL2},
        { "skill3",AbilityCastType.SKILL3},
        { "skill4",AbilityCastType.SKILL4},
        { "passive",AbilityCastType.PASSIVE},
    };
    private HeroState m_HeroState;
    private Ability m_lastAbility;
    private BTAction m_BehaviorTree;
    private BTAction m_DecisionTree;
    private BattleProperty m_Property;
    private List<D2Modifier> m_activeModifiers;
    private BattleData m_DecisionWorkData;
    private BattleData m_BehaviorWorkData;
    private Dictionary<AbilityCastType, Ability> m_abilityMap;

    public BattleUnit(BattleCamp battleCamp, BattleProperty property)
    {        
        property.id = id;
        camp = battleCamp;
        m_Property = property;
        abilities = new List<Ability>(5);
        m_abilityMap = new Dictionary<AbilityCastType, Ability>(5);
        m_HeroState = HeroState.IDLE;
        m_BehaviorTree = GetBehaviorTree();
        m_DecisionTree = GetDecisionTree();

        m_DecisionWorkData = new BattleData(this);
        m_BehaviorWorkData = new BattleData(this);

        InitAbilities();
    }

    #region 生命周期

    /// <summary>
    /// 进入战斗
    /// </summary>
    public override void OnEnter()
    {

    }

    /// <summary>
    /// 退出战斗
    /// </summary>
    public override void OnExit()
    {

    }

    /// <summary>
    /// 出生
    /// </summary>
    public void OnBorn()
    {
    }

    /// <summary>
    /// 死亡
    /// </summary>
    public void OnDead()
    {
        SetState(HeroState.DEAD);

        ClearAbilities();
        ClearModifiers();
    }

    /// <summary>
    /// 重生
    /// </summary>
    public void OnReincarnate()
    {

    }

    #endregion

    #region ability
    public void InitAbilities()
    {
        List<int> skillList = GetSkillList();
        foreach(int skillID in skillList)
        {
            //skillItem skillItem = skillTable.Instance.GetTableItem(skillID);
            Ability ability = AbilityReader.GetAbility(skillID, this);
            string skillName = ability.GetCastAnimation();
            if(string.IsNullOrEmpty(skillName))
                skillName = "passive";
            var castType = m_stringToCastTypeMap[skillName];
            m_abilityMap[castType] = ability;
            abilities.Add(ability);
        }

        m_activeModifiers = new List<D2Modifier>();

        ActivePassiveModifier();
    }

    public void ClearAbilities()
    {
        abilities.Clear();
    }

    // 找到可以释放的技能
    public Ability SelectCastableAbility()
    {
        foreach(Ability ability in abilities)
        {
            if(ability.IsCastable())
                return ability;
        }
        return null;
    }

    /// <summary>
    /// 技能准备动作：比如钟馗的甩钩子，可以取消的
    /// </summary>
    /// <param name="ability"></param>
    public void PrepareCastAbility(Ability ability)
    {
        if(ability == null)
            return;

        SetState(HeroState.REARY_CAST, ability.GetCastAnimation());
    }

    public void CastAbility(Ability ability, bool isSkipCastPoint = false)
    {
        if(ability == null)
            return;
        
        ability.CastAbilityBegin(isSkipCastPoint);
        m_lastAbility = ability;

        SetState(HeroState.CASTING, ability.GetCastAnimation(), isSkipCastPoint);

        GameMsg.instance.SendMessage(GameMsgDef.Hero_Cast_Ability, id, ability.GetCastAnimation());
    }

    public void CastAbilityAnimation(Ability ability, bool isSkipCastPoint = false)
    {
        SetState(HeroState.CASTING, ability.GetCastAnimation(), isSkipCastPoint);
    }

    public void CastAbilityEnd()
    {
        SetState(HeroState.IDLE);

        m_lastAbility = null;
    }

    #endregion

    #region modifier
    //可以理解为buff，就是对角色属性的一些修改：比如物理攻击增加

    public void ApplyModifierByName(BattleUnit caster, AbilityData abilityData, string modifierName)
    {
        var modifierData = abilityData.GetModifierData(modifierName);
        if(modifierData != null)
        {
            BattleLog.LogError("角色[{0}]的技能[{1}]未找到Modifier[{2}]的数据", id, abilityData.configFileName, modifierName);
            return;
        }

        ApplyModifier(caster, abilityData, modifierData);
    }

    public void ApplyModifier(BattleUnit caster, AbilityData abilityData, ModifierData modifierData)
    {
        var modifier = new D2Modifier(caster, modifierData, this, abilityData);
        modifier.OnCreate();

        m_activeModifiers.Add(modifier);
    }

    public void ClearModifiers()
    {
        m_activeModifiers.Clear();
    }

    // 激活被动
    private void ActivePassiveModifier()
    {
        for(int i = 0; i < abilities.Count; i++)
        {
            var ability = abilities[i];
            var modifierDatas = ability.GetAllPassiveModifierData();
            for(int j = 0; j < modifierDatas.Count; j++)
            {
                var modifierData = modifierDatas[j];
                ApplyModifier(this, ability.abilityData, modifierData);
            }
        }
    }
    #endregion

    #region 决策树、行为树、技能更新

    // 决策层：输入（游戏世界信息），输出（请求）
    // 行为层：输入（请求），输出（修改游戏世界的相关信息）

    // 设置决策
    public void ForceUpdateDecisionRequest(BehaviorRequest behaviorRequest)
    {
        m_DecisionWorkData.request = behaviorRequest;
    }

    // 更新决策树
    public int UpdateDecision(float gameTime, float deltaTime)
    {
        if(m_DecisionTree == null)
            return 1;

        if(m_DecisionTree.Evaluate(m_DecisionWorkData))
            m_DecisionTree.Update(m_DecisionWorkData);

        return 0;
    }

    // 更新请求
    public int UpdateRequest(float gameTime, float deltaTime)
    {
        var foregroundRequest = m_BehaviorWorkData.request;
        var backgroundRequest = m_DecisionWorkData.request;
        if(backgroundRequest != foregroundRequest)
        {
            //比对决策树和行为树的请求是否一致
            if(foregroundRequest != null)
                GameLog.Log("[UpdateRequest]当前请求：" + foregroundRequest.ToString());

            if(backgroundRequest != null)
                GameLog.Log("[UpdateRequest]下一个请求：" + backgroundRequest.ToString());

            //reset 清理一些状态
            m_BehaviorTree.Transition(m_BehaviorWorkData);
            //assign to current
            m_BehaviorWorkData.request = backgroundRequest;
            m_DecisionWorkData.request = null;// 消耗掉了这个请求
        }
        return 0;
    }

    // 更新行为树
    public int UpdateBehavior(float gameTime, float deltaTime)
    {
        if(m_BehaviorTree == null)
            return 1;

        BehaviorRequest currentRequest = m_BehaviorWorkData.request;
        if(currentRequest == null)
            return 1;

        //if(currentRequest.isRequestCompleted)
        //    return 1;
        
        m_BehaviorWorkData.gameTime = gameTime;
        m_BehaviorWorkData.deltaTime = deltaTime;

        if(m_BehaviorTree.Evaluate(m_BehaviorWorkData))
        {
            m_BehaviorTree.Update(m_BehaviorWorkData);
        }
  
        return 0;
    }

    // 技能刷新
    public int UpdateAbility(float gameTime, float deltaTime)
    {
        // 技能
        for(int i = 0; i < abilities.Count; i++)
        {
            abilities[i].Update(deltaTime);
        }

        // buff刷新
        for(int i = 0; i < m_activeModifiers.Count; i++)
        {
            m_activeModifiers[i].Update(deltaTime);
        }

        return 0;
    }

    #endregion

    #region get

    protected virtual BTAction GetBehaviorTree()
    {
        return null;
    }

    protected virtual BTAction GetDecisionTree()
    {
        return null;
    }

    public AbilityCastType GetCastType(string skillName)
    {
        AbilityCastType abilityCastType;
        m_stringToCastTypeMap.TryGetValue(skillName, out abilityCastType);
        return abilityCastType;
    }

    public Ability GetAbility(AbilityCastType castType)
    {
        Ability ability;
        m_abilityMap.TryGetValue(castType, out ability);
        if(ability != null)
        {
            return ability;
        }
        return null;
    }

    public bool IsCanDecision()
    {
        if(IsDead())
            return false;

        if(m_HeroState != HeroState.IDLE)
            return false;

        // todo 状态：眩晕、等状态不可决策
        return true;
    }

    public bool IsDead()
    {
        return GetHP() <= 0;
    }

    public bool IsUnSelectable()
    {
        return IsDead();
    }

    public BattleProperty GetProperty()
    {
        return m_Property;
    }

    /// <summary>
    /// 自增ID
    /// </summary>
    /// <returns></returns>
    public int GetUniqueID()
    {
        return id;
    }

    /// <summary>
    /// configID
    /// </summary>
    /// <returns></returns>
    public int GetID()
    {
        return m_Property.GetID();
    }

    public string GetName()
    {
        return string.Format("{0}[{1}]",GetID(), id);
    }

    public float GetAttackRange()
    {
        return m_Property.GetAttackRange();
    }

    public float GetMaxRadius()
    {
        return m_Property.GetMaxRadius();
    }

    public float GetViewRange()
    {
        return m_Property.GetViewRange();
    }

    public float GetMoveSpeed()
    {
        return m_Property.GetMoveSpeed();
    }

    public float GetTurnSpeed()
    {
        return m_Property.GetTurnSpeed();
    }

    public void GetStartPoint(out float x, out float y)
    {
        m_Property.GetStartPoint(out x, out y);
    }

    public Vector3 GetStartPoint()
    {
        return m_Property.GetStartPoint();
    }

    public string GetModelPath()
    {
        return m_Property.GetModelPath();
    }

    public int GetLevel()
    {
        return m_Property.Level;
    }

    public int GetHP()
    {
        return m_Property.curHP;
    }

    public float GetHPPercent()
    {
        return m_Property.curHP/m_Property.GetMaxHP();
    }

    public int GetEnergy()
    {
        return m_Property.curEnergy;
    }

    public List<int> GetSkillList()
    {
        return m_Property.GetSkillList();
    }

    /// <summary>
    /// 尝试打断技能
    /// </summary>
    /// <param name="forceBreak">如果技能处于后摇状态，false 就不管（后摇动作会继续播放,也就是技能会正常结束) true 会被停止(后摇动作会被切换，技能提前结束)</param>
    /// <param name="breakAbilityBranch">打断的技能分支：物理/魔法</param>
    public void TryBreakAbility(bool forceBreak, AbilityBranch breakAbilityBranch = default)
    {
        if(m_lastAbility !=null)
        {
            m_lastAbility.TryBreakAbility(forceBreak, breakAbilityBranch);
        }
    }

    #endregion

    #region set

    /// <summary>
    /// 更新血量
    /// </summary>
    public void UpdateHP(float damage)
    {
        m_Property.UpdateHP(damage);

        var hp = GetHP();
        if(hp <= 0)
        {
            OnDead();
        }
    }

    /// <summary>
    /// 设置角色状态
    /// </summary>
    /// <param name="state">状态枚举</param>
    /// <param name="skillName">技能动画名称</param>
    /// <param name="isSkipCastPoint">是否跳过前摇</param>
    public void SetState(HeroState state, string skillName = null, bool isSkipCastPoint = false)
    {
        m_HeroState = state;
        GameMsg.instance.SendMessage(GameMsgDef.Hero_ChangeState, id, state, skillName, isSkipCastPoint);
    }

    /// <summary>
    /// 完成当前行为
    /// </summary>
    public void SetRequestComplete()
    {
        m_BehaviorWorkData.request.SetRequestCompleteState(true);
        m_BehaviorWorkData.request = null;
    }

    #endregion
}