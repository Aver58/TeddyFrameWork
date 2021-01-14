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
/// 技能指示器 + 施法者控制
/// </summary>
public class AbilityInput
{
    private HeroActor m_casterActor;
    private Transform m_casterTransform;
    private List<HeroActor> m_targetActors;
    private List<AbilityIndicator> m_AbilityIndicators;

    public AbilityInput(HeroActor casterActor)
    {
        m_casterActor = casterActor;
        m_casterTransform = m_casterActor.transform;
        m_targetActors = new List<HeroActor>();
        m_AbilityIndicators = new List<AbilityIndicator>();
    }

    #region API
    public void AddAbilityIndicator(AbilityIndicator abilityIndicator)
    {
        m_AbilityIndicators.Add(abilityIndicator);
    }

    public void OnFingerDown()
    {
        ShowAbilityAllIndicator();
    }

    public void OnFingerDrag(Vector3 forward)
    {
        var castPos = m_casterTransform.position;
        float dragWorldPointX, dragWorldPointZ, dragForwardX, dragForwardZ;
        // todo 技能范围*delta
        OnFingerDrag(castPos.x, castPos.z, dragWorldPointX, dragWorldPointZ, dragForwardX, dragForwardZ);
    }

    public void OnFingerUp()
    {
        HideAbilityAllIndicator();
        m_targetActors.Clear();
    }
    #endregion

    #region Private

    private void OnFingerDrag(float casterX, float casterZ, float dragWorldPointX, float dragWorldPointZ, float dragForwardX, float dragForwardZ)
    {

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