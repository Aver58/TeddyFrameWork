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
    private Transform m_casterTransform;
    private AbilityInput m_abilityInput;

    public AbilityActor(Ability ability, Transform casterTransform)
    {
        m_ability = ability;
        m_casterTransform = casterTransform;
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

    // todo 移到外面，应该有一个管理器,而不是一个actor一份配置
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
            var go = Object.Instantiate(asset);
            return go.transform;
        }
        return null;
    }

    // 非指向性技能输入，妲己一技能
    private AbilityInput CreateAbilityNoTargetInput(AbilityBehavior abilityBehavior)
    {
        AbilityInput abilityInput;
        // 直线型
        if((abilityBehavior & AbilityBehavior.ABILITY_BEHAVIOR_LINE_AOE) != 0)
        {
            abilityInput = new AbilityInputDirection(m_casterTransform, m_ability);
            var trans = GetIndicatorAsset(AbilityIndicatorType.RANGE_AREA);
            var castRange = m_ability.GetCastRange();
            AbilityIndicatorRange abilityIndicatorRange = new AbilityIndicatorRange(trans, m_casterTransform, castRange);
            trans = GetIndicatorAsset(AbilityIndicatorType.LINE_AREA);
            float lineLength, lineThickness;
            m_ability.GetLineAoe(out lineLength, out lineThickness);
            AbilityIndicatorLine abilityIndicatorLine = new AbilityIndicatorLine(trans, m_casterTransform, lineLength, lineThickness);
            abilityInput.AddAbilityIndicator(abilityIndicatorRange);
            abilityInput.AddAbilityIndicator(abilityIndicatorLine);
            return abilityInput;
        }

        // 扇形型
        if((abilityBehavior & AbilityBehavior.ABILITY_BEHAVIOR_SECTOR_AOE) != 0)
        {
            abilityInput = new AbilityInputDirection(m_casterTransform, m_ability);
            AbilityIndicatorType abilityIndicatorType;
            float sectorRadius, sectorAngle;
            m_ability.GetSectorAoe(out sectorRadius, out sectorAngle);
            if(sectorAngle == 60)
            {
                abilityIndicatorType = AbilityIndicatorType.SECTOR60_AREA;
            }
            else if(sectorAngle == 90)
            {
                abilityIndicatorType = AbilityIndicatorType.SECTOR90_AREA;
            }
            else if(sectorAngle == 120)
            {
                abilityIndicatorType = AbilityIndicatorType.SECTOR120_AREA;
            }
            else
            {
                BattleLog.LogError("技能[%s]中有未定义的扇形AOE范围角度[%s]", m_ability.GetConfigName(), sectorAngle);
            }

            var trans = GetIndicatorAsset(AbilityIndicatorType.LINE_AREA);
            var abilityIndicatorSector = new AbilityIndicatorLine(trans, m_casterTransform, sectorRadius, sectorRadius);

            AbilityIndicatorLine abilityIndicatorLine = new AbilityIndicatorLine(trans, m_casterTransform, sectorRadius, sectorRadius);
            abilityInput.AddAbilityIndicator(abilityIndicatorSector);
            return abilityInput;
        }

        // 范围型
        abilityInput = new AbilityInputDirection(m_casterTransform, m_ability);
        if((abilityBehavior & AbilityBehavior.ABILITY_BEHAVIOR_RADIUS_AOE) != 0)
        {
            var trans = GetIndicatorAsset(AbilityIndicatorType.RANGE_AREA);
            AbilityIndicatorRange abilityIndicatorRange = new AbilityIndicatorRange(trans, m_casterTransform, m_ability.GetCastRange());
            abilityInput.AddAbilityIndicator(abilityIndicatorRange);
        }

        return abilityInput;
    }

    // 点施法类型，王昭君大招
    private AbilityInput CreateAbilityPointInput(AbilityBehavior abilityBehavior)
    {
        var abilityInput = new AbilityInputPoint(m_casterTransform, m_ability);
        if((abilityBehavior & AbilityBehavior.ABILITY_BEHAVIOR_RADIUS_AOE) != 0)
        {
            var radius = m_ability.GetAbilityAOERadius();
            var trans = GetIndicatorAsset(AbilityIndicatorType.RANGE_AREA);
            AbilityIndicatorRange abilityIndicatorRange = new AbilityIndicatorRange(trans, m_casterTransform, m_ability.GetCastRange());
            trans = GetIndicatorAsset(AbilityIndicatorType.CIRCLE_AREA);
            AbilityIndicatorPoint abilityIndicatorPoint = new AbilityIndicatorPoint(trans, m_casterTransform, radius);
            abilityInput.AddAbilityIndicator(abilityIndicatorRange);
            abilityInput.AddAbilityIndicator(abilityIndicatorPoint);
        }
        else
            BattleLog.LogError("技能[%s]中有未定义的Point类型技能区域", m_ability.GetConfigName());

        return abilityInput;
    }

    // 指向性技能输入：妲己二技能
    private AbilityInput CreateAbilityTargetInput(AbilityBehavior abilityBehavior)
    {
        var abilityInput = new AbilityInputPoint(m_casterTransform, m_ability);
        var trans = GetIndicatorAsset(AbilityIndicatorType.RANGE_AREA);
        AbilityIndicatorRange abilityIndicatorRange = new AbilityIndicatorRange(trans, m_casterTransform, m_ability.GetCastRange());
        trans = GetIndicatorAsset(AbilityIndicatorType.SEGMENT);
        AbilityIndicatorSegment abilityIndicatorSegment = new AbilityIndicatorSegment(trans, m_casterTransform);
        var radius = m_ability.GetAbilityAOERadius();
        trans = GetIndicatorAsset(AbilityIndicatorType.SEGMENT_AREA);
        AbilityIndicatorPoint abilityIndicatorPoint = new AbilityIndicatorPoint(trans, m_casterTransform, radius);
        abilityInput.AddAbilityIndicator(abilityIndicatorRange);
        abilityInput.AddAbilityIndicator(abilityIndicatorSegment);
        abilityInput.AddAbilityIndicator(abilityIndicatorPoint);
        return abilityInput;
    }

    //解析技能行为
    private AbilityInput CreateAbilityInput(Ability ability)
    {
        AbilityBehavior abilityBehavior = ability.GetAbilityBehavior();
        if((abilityBehavior & AbilityBehavior.ABILITY_BEHAVIOR_NO_TARGET) != 0)
            return CreateAbilityNoTargetInput(abilityBehavior);

        if((abilityBehavior & AbilityBehavior.ABILITY_BEHAVIOR_UNIT_TARGET) != 0)
            return CreateAbilityPointInput(abilityBehavior);

        if((abilityBehavior & AbilityBehavior.ABILITY_BEHAVIOR_POINT) != 0)
            return CreateAbilityTargetInput(abilityBehavior);

        BattleLog.LogError("技能[%s]中有未定义的Input类型", ability.GetConfigName());
        return null;
    }

    #endregion
}