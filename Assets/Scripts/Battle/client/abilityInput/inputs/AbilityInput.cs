#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    AbilityInput.cs
 Author:      Zeng Zhiwei
 Time:        2021\1\10 星期日 17:55:35
=====================================================
*/
#endregion

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 技能输入抽象类
/// 更新技能指示器 + 更新施法者朝向
/// </summary>
public abstract class AbilityInput
{
    protected Ability m_Ability;
    protected HeroActor m_CasterActor;
    protected Vector3 m_CacheVector = Vector3.zero;

    private bool m_IsManualSelect = false;
    private List<AbilityIndicator> m_AbilityIndicators;

    public AbilityInput(Ability ability, HeroActor casterActor)
    {
        m_Ability = ability;
        m_CasterActor = casterActor;
        m_AbilityIndicators = new List<AbilityIndicator>();
        m_IsManualSelect = false;
    }

    #region API

    public void AddAbilityIndicator(AbilityIndicator abilityIndicator)
    {
        m_AbilityIndicators.Add(abilityIndicator);
    }

    public virtual void OnFingerDown()
    {
        ShowAbilityAllIndicator();
        // 初始化目标
        //battleUnit.target = TargetSearcher.instance.FindTargetUnitByAbility(battleUnit, ability);
    }

    public virtual void OnFingerDrag(float casterX, float casterZ, float dragWorldPointX, float dragWorldPointZ, float dragForwardX, float dragForwardZ)
    {
        m_IsManualSelect = true;
        //更新施法者朝向
        m_Ability.caster.Set2DForward(dragForwardX, dragForwardZ);
        m_CacheVector.Set(dragForwardX, 0, dragForwardZ);
        if(m_CacheVector != Vector3.zero)
            m_CasterActor.Set3DForward(m_CacheVector);

        //更新技能指示器
        for(int i = 0; i < m_AbilityIndicators.Count; i++)
            m_AbilityIndicators[i].Update(casterX, casterZ, dragWorldPointX, dragWorldPointZ, dragForwardX, dragForwardZ);
    }

    public virtual void OnFingerUp()
    {
        HideAbilityAllIndicator();

        TriggerSkill();
    }

    #endregion

    #region Protected

    protected virtual void OnManualCastSkill(){}

    #endregion

    #region Private

    private void TriggerSkill()
    {
        if(m_IsManualSelect)
        {
            OnManualCastSkill();
        }
        else
        {
            // AI选择对象
            GameLog.Log("AI选择对象！");
            OnCastSkillByAI();
        }
    }

    /// <summary>
    /// 根据配置，自动选择目标
    /// </summary>
    private void OnCastSkillByAI()
    {
        var target = TargetSearcher.Instance.FindTargetUnitByAbilityRange(m_CasterActor.battleUnit, m_Ability);
        if(target == null)
        {
            GameLog.Log("ULT技能[{0}]AI未找到目标?", m_Ability.GetConfigName());
            return;
        }

        m_Ability.SetUnitTarget(target);
        var request = new ManualCastAbilityRequest(m_Ability);
        m_CasterActor.ForceUpdateDecisionRequest(request);
    }

    private void ShowAbilityAllIndicator()
    {
        for(int i = 0; i < m_AbilityIndicators.Count; i++)
        {
            m_AbilityIndicators[i].Show();
        }
    }

    private void HideAbilityAllIndicator()
    {
        for(int i = 0; i < m_AbilityIndicators.Count; i++)
        {
            m_AbilityIndicators[i].Hide();
        }
    }

    #endregion
}