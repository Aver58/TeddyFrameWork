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
    protected Ability m_ability;
    protected Vector3 m_cacheVector = Vector3.zero;
    protected Transform m_casterTransform;
    protected List<HeroActor> m_targetActors;
    private List<AbilityIndicator> m_AbilityIndicators;

    public AbilityInput(Transform casterTransform, Ability ability)
    {
        m_ability = ability;
        m_casterTransform = casterTransform;
        m_targetActors = new List<HeroActor>();
        m_AbilityIndicators = new List<AbilityIndicator>();
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
        //更新施法者朝向
        m_ability.caster.Set2DForward(dragForwardX, dragForwardZ);
        m_cacheVector.Set(dragForwardX, 0, dragForwardZ);
        if(m_cacheVector != Vector3.zero)
            m_casterTransform.forward = m_cacheVector;

        //更新技能指示器
        for(int i = 0; i < m_AbilityIndicators.Count; i++)
            m_AbilityIndicators[i].Update(casterX, casterZ, dragWorldPointX, dragWorldPointZ, dragForwardX, dragForwardZ);

        // 更新目标，根据区域选择敌人
        //Ability ability = battleUnit.GetAbility(castType);
        //battleUnit.target = TargetSearcher.instance.FindTargetUnitByAbility(battleUnit, ability);
    }

    public virtual void OnFingerUp()
    {
        HideAbilityAllIndicator();

        TriggerSkill();
        m_targetActors.Clear();
    }

    #endregion

    #region Protected

    protected virtual void OnManualCastSkill(){}

    #endregion

    #region Private

    private void TriggerSkill()
    {
        OnManualCastSkill();
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