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

using System.Collections.Generic;
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
        m_abilityInput = CreateAbilityInput(ability);
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

    // todo 移到外面，应该有一个管理器
    private static Dictionary<AbilityIndicatorType, string> AbilityIndicatorAssetConfig = new Dictionary<AbilityIndicatorType, string>
    {
        {AbilityIndicatorType.CIRCLE_AREA,"rpgBattle/P_CircleArea" },
        {AbilityIndicatorType.LINE_AREA,"rpgBattle/P_LineArea" },
        {AbilityIndicatorType.SECTOR60_AREA,"rpgBattle/P_Sector60Area" },
        {AbilityIndicatorType.SECTOR90_AREA,"rpgBattle/P_Sector90Area" },
        {AbilityIndicatorType.SECTOR120_AREA,"rpgBattle/P_Sector120Area" },
        {AbilityIndicatorType.RANGE_AREA,"rpgBattle/P_RangeArea" },
        {AbilityIndicatorType.SEGMENT,"rpgBattle/P_Segment" },
        {AbilityIndicatorType.SEGMENT_AREA,"rpgBattle/P_SegmentArea" },
    };

    private Transform GetIndicatorAsset(AbilityIndicatorType abilityIndicatorType)
    {
        string path = string.Empty;
        AbilityIndicatorAssetConfig.TryGetValue(abilityIndicatorType,out path);
        if(!string.IsNullOrEmpty(path))
        {
            var request = LoadModule.LoadAsset(path,typeof(GameObject));
            var asset = request.asset as GameObject;
            var go = Object.Instantiate<GameObject>(asset);
            return go.transform;
        }
        return null;
    }

    private AbilityInput CreateAbilityNoTargetInput()
    {
        var abilityInput = new AbilityInputPoint();
        return abilityInput;
    }

    // 点施法类型，例如王昭君的大招
    private AbilityInput CreateAbilityPointInput(AbilityBehavior abilityBehavior)
    {
        var abilityInput = new AbilityInputPoint();
        if((abilityBehavior & AbilityBehavior.ABILITY_BEHAVIOR_RADIUS_AOE) != 0)
        {
            var radius = m_ability.GetAbilityAOERadius();
            var trans = GetIndicatorAsset(AbilityIndicatorType.RANGE_AREA);
            AbilityIndicatorRange abilityIndicatorRange = new AbilityIndicatorRange(trans,m_ability.GetCastRange());
            AbilityIndicatorPoint abilityIndicatorPoint = new AbilityIndicatorPoint(trans, radius);
            abilityInput.AddAbilityIndicator(abilityIndicatorRange);
            abilityInput.AddAbilityIndicator(abilityIndicatorPoint);
        }
        else
        {
            BattleLog.LogError("技能[%s]中有未定义的Point类型技能区域", m_ability.GetConfigName());
        }

        return abilityInput;
    }

    private AbilityInput CreateAbilityTargetInput()
    {
        var abilityInput = new AbilityInputPoint();
        return abilityInput;
    }

    //解析技能行为
    private AbilityInput CreateAbilityInput(Ability ability)
    {
        AbilityBehavior abilityBehavior = ability.GetAbilityBehavior();
        if((abilityBehavior & AbilityBehavior.ABILITY_BEHAVIOR_NO_TARGET) != 0)
            return CreateAbilityNoTargetInput();

        if((abilityBehavior & AbilityBehavior.ABILITY_BEHAVIOR_UNIT_TARGET) != 0)
            return CreateAbilityPointInput(abilityBehavior);


        if((abilityBehavior & AbilityBehavior.ABILITY_BEHAVIOR_POINT) != 0)
            return CreateAbilityTargetInput();

        BattleLog.LogError("技能[%s]中有未定义的Input类型", ability.GetConfigName());
        return null;
    }

    #endregion
}