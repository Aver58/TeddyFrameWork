#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    AbilityActor.cs
 Author:      Zeng Zhiwei
 Time:        2021\1\10 星期日 20:35:23
=====================================================
*/
#endregion

using UnityEngine;

/// <summary>
/// 技能Actor【技能指示器、特效】
/// </summary>
public class AbilityActor
{
    private Ability m_ability;
    private AbilityInput m_abilityInput;

    public AbilityActor(Ability ability)
    {
        m_ability = ability;
        // 解析技能指示器
        //m_abilityInput = new AbilityInput();
    }

    public void OnFingerDown()
    {
        if(m_abilityInput != null)
            m_abilityInput.OnFingerDown();
    }

    public void OnFingerDrag(Vector3 forward)
    {
        if(m_abilityInput != null)
            m_abilityInput.OnFingerDrag(forward);
    }

    public void OnFingerUp()
    {
        if(m_abilityInput != null)
            m_abilityInput.OnFingerUp();
    }

    #region Private

    private AbilityInput CreateAbilityNoTargetInput()
    {

    }

    // 点施法类型，例如王昭君的大招
    private AbilityInput CreateAbilityPointInput()
    {
        var abilityInput = new AbilityInputPoint();

    }

    private AbilityInput CreateAbilityTargetInput()
    {

    }

    //解析技能行为
    private AbilityInput CreateAbilityInput()
    {
        AbilityBehavior abilityBehavior = m_ability.GetAbilityBehavior();
        if((abilityBehavior & AbilityBehavior.ABILITY_BEHAVIOR_NO_TARGET) != 0)
            return CreateAbilityNoTargetInput();

        if((abilityBehavior & AbilityBehavior.ABILITY_BEHAVIOR_UNIT_TARGET) != 0)
            return CreateAbilityPointInput();


        if((abilityBehavior & AbilityBehavior.ABILITY_BEHAVIOR_POINT) != 0)
            return CreateAbilityTargetInput();

        BattleLog.LogError("技能[%s]中有未定义的Input类型", m_ability.GetConfigName());
        return null;
    }

    #endregion
}