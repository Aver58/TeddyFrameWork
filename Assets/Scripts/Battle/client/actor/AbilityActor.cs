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
    private float m_radius;
    public Ability ability;
    private Transform m_casterTransform;
    private AbilityInput m_abilityInput;

    public AbilityActor(Ability ability, Transform casterTransform)
    {
        this.ability = ability;
        m_radius = ability.GetCastRange();
        m_casterTransform = casterTransform;
        // 解析技能指示器
        m_abilityInput = CreateAbilityInput(ability);
    }

    public void OnFingerDown()
    {
        if(m_abilityInput != null)
            m_abilityInput.OnFingerDown();
    }

    public void OnFingerDrag(Vector2 mouseDelta)
    {
        if(m_abilityInput != null)
        {
            var casterPos = m_casterTransform.position;
            float dragWorldPointX, dragWorldPointZ, dragForwardX, dragForwardZ;

            // z轴正方向为up 技能范围*delta
            dragWorldPointX = casterPos.x + m_radius * mouseDelta.x;
            dragWorldPointZ = casterPos.z + m_radius * mouseDelta.y;
            dragForwardX = dragWorldPointX - casterPos.x;
            dragForwardZ = dragWorldPointZ - casterPos.z;
            m_abilityInput.OnFingerDrag(casterPos.x, casterPos.z, dragWorldPointX, dragWorldPointZ, dragForwardX, dragForwardZ);
        }
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
        if((abilityBehavior & AbilityBehavior.ABILITY_BEHAVIOR_LINE_AOE) != 0)
        {
            abilityInput = new AbilityInputDirection(m_casterTransform, ability);
            var castRange = ability.GetCastRange();
            abilityIndicatorRange = new AbilityIndicatorRange(rangeTrans, m_casterTransform, castRange);
            var trans = GetIndicatorAsset(AbilityIndicatorType.LINE_AREA);
            float lineLength, lineThickness;
            ability.GetLineAoe(out lineLength, out lineThickness);
            AbilityIndicatorLine abilityIndicatorLine = new AbilityIndicatorLine(trans, m_casterTransform, lineLength, lineThickness);
            abilityInput.AddAbilityIndicator(abilityIndicatorRange);
            abilityInput.AddAbilityIndicator(abilityIndicatorLine);
            return abilityInput;
        }

        // 扇形型
        if((abilityBehavior & AbilityBehavior.ABILITY_BEHAVIOR_SECTOR_AOE) != 0)
        {
            abilityInput = new AbilityInputDirection(m_casterTransform, ability);
            AbilityIndicatorType abilityIndicatorType;
            float sectorRadius, sectorAngle;
            ability.GetSectorAoe(out sectorRadius, out sectorAngle);
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
                BattleLog.LogError("技能[{0}]中有未定义的扇形AOE范围角度[{1}]", ability.GetConfigName(), sectorAngle);
                return null;
            }

            var castRange = ability.GetCastRange();
            abilityIndicatorRange = new AbilityIndicatorRange(rangeTrans, m_casterTransform, castRange);

            var trans = GetIndicatorAsset(abilityIndicatorType);
            var abilityIndicatorSector = new AbilityIndicatorLine(trans, m_casterTransform, sectorRadius, sectorRadius);
         
            abilityInput.AddAbilityIndicator(abilityIndicatorRange);
            abilityInput.AddAbilityIndicator(abilityIndicatorSector);
            return abilityInput;
        }

        // 范围型AOE 和 普通攻击，可以原地平A
        abilityInput = new AbilityInputDirection(m_casterTransform, ability);
        abilityIndicatorRange = new AbilityIndicatorRange(rangeTrans, m_casterTransform, ability.GetCastRange());
        abilityInput.AddAbilityIndicator(abilityIndicatorRange);

        return abilityInput;
    }

    // 点施法类型，王昭君大招
    private AbilityInput CreateAbilityPointInput(AbilityBehavior abilityBehavior)
    {
        var abilityInput = new AbilityInputPoint(m_casterTransform, ability);
        if((abilityBehavior & AbilityBehavior.ABILITY_BEHAVIOR_RADIUS_AOE) != 0)
        {
            var radius = ability.GetAbilityAOERadius();
            var trans = GetIndicatorAsset(AbilityIndicatorType.RANGE_AREA);
            AbilityIndicatorRange abilityIndicatorRange = new AbilityIndicatorRange(trans, m_casterTransform, ability.GetCastRange());
            trans = GetIndicatorAsset(AbilityIndicatorType.CIRCLE_AREA);
            AbilityIndicatorPoint abilityIndicatorPoint = new AbilityIndicatorPoint(trans, m_casterTransform, radius);

            abilityInput.AddAbilityIndicator(abilityIndicatorRange);
            abilityInput.AddAbilityIndicator(abilityIndicatorPoint);
        }
        else
            BattleLog.LogError("技能[{0}]中有未定义的Point类型技能区域", ability.GetConfigName());

        return abilityInput;
    }

    // 指向性技能输入：妲己二技能
    private AbilityInput CreateAbilityTargetInput(AbilityBehavior abilityBehavior)
    {
        var abilityInput = new AbilityInputPoint(m_casterTransform, ability);
        var trans = GetIndicatorAsset(AbilityIndicatorType.RANGE_AREA);
        AbilityIndicatorRange abilityIndicatorRange = new AbilityIndicatorRange(trans, m_casterTransform, ability.GetCastRange());
        trans = GetIndicatorAsset(AbilityIndicatorType.SEGMENT);
        AbilityIndicatorSegment abilityIndicatorSegment = new AbilityIndicatorSegment(trans, m_casterTransform);
        var radius = ability.GetAbilityAOERadius();
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
        // 非指向性技能，妲己一技能
        AbilityBehavior abilityBehavior = ability.GetAbilityBehavior();
        if((abilityBehavior & AbilityBehavior.ABILITY_BEHAVIOR_NO_TARGET) != 0)
            return CreateAbilityNoTargetInput(abilityBehavior);

        // 指向性技能：妲己二技能
        if((abilityBehavior & AbilityBehavior.ABILITY_BEHAVIOR_UNIT_TARGET) != 0)
            return CreateAbilityTargetInput(abilityBehavior);

        // 点施法类型，王昭君大招
        if((abilityBehavior & AbilityBehavior.ABILITY_BEHAVIOR_POINT) != 0)
            return CreateAbilityPointInput(abilityBehavior);

        BattleLog.LogError("技能[%s]中有未定义的Input类型", ability.GetConfigName());
        return null;
    }

    #endregion
}