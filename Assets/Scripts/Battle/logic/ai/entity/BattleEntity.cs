﻿#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    GuardUnit.cs
 Author:      Zeng Zhiwei
 Time:        2020/3/5 13:43:29
=====================================================
*/
#endregion

using System;
using System.Collections.Generic;
using TsiU;
using UnityEngine;

public class BattleEntity : Entity
{
    public static Dictionary<string,AbilityCastType> animationName2CastTypeMap = new Dictionary<string, AbilityCastType> {
        { "attack",AbilityCastType.ATTACK},
        { "skill1",AbilityCastType.SKILL1},
        { "skill2",AbilityCastType.SKILL2},
        { "skill3",AbilityCastType.SKILL3},
    };

    public BattleCamp camp { get; }
    public BattleCamp enemyCamp { get;set; }
    public BattleEntity target { get; set; }
    public List<Ability> abilities;
    private Dictionary<AbilityCastType, Ability> abilityMap;

    private HeroState m_HeroState;
    private TBTAction m_BehaviorTree;
    private TBTAction m_DecisionTree;
    private BattleProperty m_Property;
    private BattleDecisionWorkingData m_DecisionWorkData;
    private BattleBehaviorWorkingData m_BehaviorWorkData;

    public BattleEntity(int id, BattleCamp battleCamp, BattleProperty property) : base(id)
    {
        property.id = id;
        camp = battleCamp;
        m_Property = property;
        abilities = new List<Ability>();
        abilityMap = new Dictionary<AbilityCastType, Ability>(4);
        m_HeroState = HeroState.IDLE;
        m_BehaviorTree = GetBehaviorTree();
        m_DecisionTree = GetDecisionTree();

        m_DecisionWorkData = new BattleDecisionWorkingData(this);
        m_BehaviorWorkData = new BattleBehaviorWorkingData(this);

        InitAbilities();
    }

    #region ability
    public void InitAbilities()
    {
        List<int> skillList = m_Property.GetSkillList();
        foreach(int skillID in skillList)
        {
            skillItem skillItem = skillTable.Instance.GetTableItem(skillID);
            //string skillConfig = skillItem.config;
            Ability ability = AbilityReader.CreateAbility(skillID, this);
            string skillName = ability.GetCastAnimation();
            var castType = animationName2CastTypeMap[skillName];
            abilityMap[castType] = ability;
            abilities.Add(ability);
        }
    }

    // 找到可以释放的技能
    public Ability SelectCastableAbility()
    {
        foreach(Ability ability in abilities)
        {
            if(ability.IsCastable())
            {
                return ability;
            }
        }
        return null;
    }

    public void CastAbility(Ability ability, bool isSkipCastPoint = false)
    {
        if(ability == null)
            return;
        
        ability.CastAbilityBegin(isSkipCastPoint);

        SetState(HeroState.CASTING,ability.GetCastAnimation(), isSkipCastPoint);
    }

    #endregion

    #region 技能、决策树、行为树更新

    // 决策层：输入（游戏世界信息），输出（请求）
    // 行为层：输入（请求），输出（修改游戏世界的相关信息）

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
        AIBehaviorRequest foregroundRequest = m_BehaviorWorkData.request;
        AIBehaviorRequest backgroundRequest = m_DecisionWorkData.request;
        if(backgroundRequest != foregroundRequest)
        {
            //比对决策树和行为树的请求是否一致
            if(foregroundRequest != null)
            {
                Debug.Log("【UpdateRequest】当前在处理的请求（前端）：" + foregroundRequest.ToString());
            }

            if(backgroundRequest != null)
            {
                Debug.Log("【UpdateRequest】下一个要处理的请求（后端）：" + backgroundRequest.ToString());
            }
            //reset bev tree 清理一些状态
            m_BehaviorTree.Transition(m_BehaviorWorkData);
            //assign to current
            m_BehaviorWorkData.request = backgroundRequest;
        }
        return 0;
    }

    // 更新行为树
    public int UpdateBehavior(float gameTime, float deltaTime)
    {
        if(m_BehaviorTree == null)
            return 1;

        AIBehaviorRequest currentRequest = m_BehaviorWorkData.request;

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
        foreach(Ability ability in abilities)
        {
            ability.Update(deltaTime);
        }
        return 0;
    }

    #endregion

    #region get interface

    protected virtual TBTAction GetBehaviorTree()
    {
        return null;
    }

    protected virtual TBTAction GetDecisionTree()
    {
        return null;
    }

    public Ability GetAbility(AbilityCastType castType)
    {
        Ability ability;
        abilityMap.TryGetValue(castType, out ability);
        if(ability != null)
        {
            return ability;
        }
        return null;
    }

    public bool IsCanDecision()
    {
        // todo 状态：眩晕、等状态不可决策
        return true;
    }

    public bool IsDead()
    {
        //return todo 
        return false;
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
    #endregion

    #region set interface

    public void UpdateHP(float damage)
    {
        m_Property.UpdateHP(damage);
    }

    public void SetState(HeroState state, string skillName = null, bool isSkipCastPoint = false)
    {
        m_HeroState = state;
        GameMsg.instance.SendMessage(GameMsgDef.Hero_ChangeState,new HeorChangeStateEventArgs(id,state, skillName, isSkipCastPoint));
    }

    #endregion
}