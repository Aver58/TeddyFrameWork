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
    private float m_radius;
    protected Ability m_ability;
    private Transform m_casterTransform;
    private List<HeroActor> m_targetActors;
    private List<AbilityIndicator> m_AbilityIndicators;

    public AbilityInput(Transform casterTransform, Ability ability)
    {
        m_radius = ability.GetCastRange();
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
    }

    public virtual void OnFingerDrag(Vector3 forward)
    {
        var casterPos = m_casterTransform.position;
        float dragWorldPointX, dragWorldPointZ, dragForwardX, dragForwardZ;

        // z轴正方向为up 技能范围*delta
        dragWorldPointX = casterPos.x + m_radius * forward.x;
        dragWorldPointZ = casterPos.z + m_radius * forward.y;
        dragForwardX = dragWorldPointX - casterPos.x;
        dragForwardZ = dragWorldPointZ - casterPos.z;
        //OnFingerDrag(casterPos.x, casterPos.z, dragWorldPointX, dragWorldPointZ, dragForwardX, dragForwardZ);

        for(int i = 0; i < m_AbilityIndicators.Count; i++)
            m_AbilityIndicators[i].Update();
    }

    public virtual void OnFingerUp()
    {
        HideAbilityAllIndicator();
        m_targetActors.Clear();
    }
    #endregion

    #region Private
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