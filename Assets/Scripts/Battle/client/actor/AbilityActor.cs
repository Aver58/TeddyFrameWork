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
    private Ability m_Ability;
    private float m_Radius;
    private HeroActor m_CasterActor;
    private AbilityInput m_AbilityInput;

    public AbilityActor(Ability ability, HeroActor casterActor)
    {
        this.m_Ability = ability;
        m_CasterActor = casterActor;
        m_Radius = ability.GetCastRange();
        // 解析技能指示器
        m_AbilityInput = CreateAbilityInput(ability);
    }

    public void OnFingerDown()
    {
        if(m_AbilityInput != null)
            m_AbilityInput.OnFingerDown();
    }

    public void OnFingerDrag(Vector2 mouseDelta)
    {
        if(m_AbilityInput != null)
        {
            var casterPos = m_CasterActor.Get3DPosition();
            float dragWorldPointX, dragWorldPointZ, dragForwardX, dragForwardZ;

            // z轴正方向为up 技能范围*delta
            dragWorldPointX = casterPos.x + m_Radius * mouseDelta.x;
            dragWorldPointZ = casterPos.z + m_Radius * mouseDelta.y;
            dragForwardX = dragWorldPointX - casterPos.x;
            dragForwardZ = dragWorldPointZ - casterPos.z;
            m_AbilityInput.OnFingerDrag(casterPos.x, casterPos.z, dragWorldPointX, dragWorldPointZ, dragForwardX, dragForwardZ);
        }
    }

    public void OnFingerUp()
    {
        if(m_AbilityInput != null)
            m_AbilityInput.OnFingerUp();
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

    private static string IndicatorPrefix = "Assets/Data/scene/";
    private static string Indicatorsuffix = ".prefab";
    private Transform GetIndicatorAsset(AbilityIndicatorType abilityIndicatorType)
    {
        string path = string.Empty;
        AbilityIndicatorAssetConfig.TryGetValue(abilityIndicatorType,out path);
        if(!string.IsNullOrEmpty(path))
        {
            path = IndicatorPrefix + path + Indicatorsuffix;
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
        AbilityIndicatorRange abilityIndicatorRange;
        var rangeTrans = GetIndicatorAsset(AbilityIndicatorType.RANGE_AREA);

        // 直线型
        if((abilityBehavior & AbilityBehavior.DOTA_ABILITY_BEHAVIOR_DIRECTIONAL) != 0)
        {
            abilityInput = new AbilityInputDirection(m_Ability, m_CasterActor);
            var castRange = m_Ability.GetCastRange();
            abilityIndicatorRange = new AbilityIndicatorRange(rangeTrans, m_CasterActor.transform, castRange);
            var trans = GetIndicatorAsset(AbilityIndicatorType.LINE_AREA);
            float lineLength, lineThickness;
            m_Ability.GetLineAoe(out lineLength, out lineThickness);
            AbilityIndicatorLine abilityIndicatorLine = new AbilityIndicatorLine(trans, m_CasterActor.transform, lineLength, lineThickness);
            abilityInput.AddAbilityIndicator(abilityIndicatorRange);
            abilityInput.AddAbilityIndicator(abilityIndicatorLine);
            return abilityInput;
        }

        // 扇形型
        if((abilityBehavior & AbilityBehavior.ABILITY_BEHAVIOR_SECTOR_AOE) != 0)
        {
            abilityInput = new AbilityInputDirection(m_Ability, m_CasterActor);
            AbilityIndicatorType abilityIndicatorType;
            float sectorRadius, sectorAngle;
            m_Ability.GetSectorAoe(out sectorRadius, out sectorAngle);
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
                BattleLog.LogError("技能[{0}]中有未定义的扇形AOE范围角度[{1}]", m_Ability.GetConfigName(), sectorAngle);
                return null;
            }

            var castRange = m_Ability.GetCastRange();
            abilityIndicatorRange = new AbilityIndicatorRange(rangeTrans, m_CasterActor.transform, castRange);

            var trans = GetIndicatorAsset(abilityIndicatorType);
            var abilityIndicatorSector = new AbilityIndicatorLine(trans, m_CasterActor.transform, sectorRadius, sectorRadius);
         
            abilityInput.AddAbilityIndicator(abilityIndicatorRange);
            abilityInput.AddAbilityIndicator(abilityIndicatorSector);
            return abilityInput;
        }

        // 范围型AOE 和 普通攻击，可以原地平A
        abilityInput = new AbilityInputDirection(m_Ability, m_CasterActor);
        abilityIndicatorRange = new AbilityIndicatorRange(rangeTrans, m_CasterActor.transform, m_Ability.GetCastRange());
        abilityInput.AddAbilityIndicator(abilityIndicatorRange);

        return abilityInput;
    }

    // 点施法类型，王昭君大招
    private AbilityInput CreateAbilityPointInput(AbilityBehavior abilityBehavior)
    {
        var abilityInput = new AbilityInputPoint(m_Ability, m_CasterActor);
        if((abilityBehavior & AbilityBehavior.ABILITY_BEHAVIOR_RADIUS_AOE) != 0)
        {
            var radius = m_Ability.GetAbilityAOERadius();
            var trans = GetIndicatorAsset(AbilityIndicatorType.RANGE_AREA);
            AbilityIndicatorRange abilityIndicatorRange = new AbilityIndicatorRange(trans, m_CasterActor.transform, m_Ability.GetCastRange());
            trans = GetIndicatorAsset(AbilityIndicatorType.CIRCLE_AREA);
            AbilityIndicatorPoint abilityIndicatorPoint = new AbilityIndicatorPoint(trans, m_CasterActor.transform, radius);

            abilityInput.AddAbilityIndicator(abilityIndicatorRange);
            abilityInput.AddAbilityIndicator(abilityIndicatorPoint);
        }
        else
            BattleLog.LogError("技能[{0}]中有未定义的Point类型技能区域", m_Ability.GetConfigName());

        return abilityInput;
    }

    // 指向性技能输入：妲己二技能
    private AbilityInput CreateAbilityTargetInput(AbilityBehavior abilityBehavior)
    {
        var abilityInput = new AbilityInputPoint(m_Ability, m_CasterActor);
        var trans = GetIndicatorAsset(AbilityIndicatorType.RANGE_AREA);
        AbilityIndicatorRange abilityIndicatorRange = new AbilityIndicatorRange(trans, m_CasterActor.transform, m_Ability.GetCastRange());
        trans = GetIndicatorAsset(AbilityIndicatorType.SEGMENT);
        AbilityIndicatorSegment abilityIndicatorSegment = new AbilityIndicatorSegment(trans, m_CasterActor.transform);
        var radius = m_Ability.GetAbilityAOERadius();
        trans = GetIndicatorAsset(AbilityIndicatorType.SEGMENT_AREA);
        AbilityIndicatorPoint abilityIndicatorPoint = new AbilityIndicatorPoint(trans, m_CasterActor.transform, radius);
        abilityInput.AddAbilityIndicator(abilityIndicatorRange);
        abilityInput.AddAbilityIndicator(abilityIndicatorSegment);
        abilityInput.AddAbilityIndicator(abilityIndicatorPoint);
        return abilityInput;
    }

    //解析技能行为
    private AbilityInput CreateAbilityInput(Ability ability)
    {
        // 非指向性技能，妲己一技能
        AbilityBehavior abilityBehavior = ability.GetAbilityBehavior();
        if((abilityBehavior & AbilityBehavior.DOTA_ABILITY_BEHAVIOR_NO_TARGET) != 0)
            return CreateAbilityNoTargetInput(abilityBehavior);

        // 指向性技能：妲己二技能
        if((abilityBehavior & AbilityBehavior.DOTA_ABILITY_BEHAVIOR_UNIT_TARGET) != 0)
            return CreateAbilityTargetInput(abilityBehavior);

        // 点施法类型，王昭君大招
        if((abilityBehavior & AbilityBehavior.DOTA_ABILITY_BEHAVIOR_POINT) != 0)
            return CreateAbilityPointInput(abilityBehavior);

        BattleLog.LogError("技能[%s]中有未定义的Input类型", ability.GetConfigName());
        return null;
    }

    #endregion
}