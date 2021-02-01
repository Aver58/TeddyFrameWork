#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    TargetSearcher.cs
 Author:      Zeng Zhiwei
 Time:        2020/5/19 9:32:10
=====================================================
*/
#endregion

using System;
using System.Collections.Generic;
using UnityEngine;

public delegate bool IsInRange(Vector2 sourcePos, Vector2 targetPos, float viewRange);

public class TargetSearcher : Singleton<TargetSearcher>
{
    // 寻找离指定施放者最近的单位
    public BattleUnit FindNearestEnemyUnit(BattleUnit caster)
    {
        BattleUnit targetEntity = null;
        // 找出敌对阵营
        BattleCamp enemyCamp = caster.enemyCamp;
        // 获取指定标记的所有单位
        List<BattleUnit> entities = BattleEntityManager.instance.GetEntities(enemyCamp);
        // 比较距离，找出最近单位
        float minDistance = Int32.MaxValue;
        foreach(BattleUnit entity in entities)
        {
            Vector2 distance = caster.Get2DPosition() - entity.Get2DPosition();
            if(distance.magnitude < minDistance)
            {
                targetEntity = entity;
                minDistance = distance.magnitude;
            }
        }

        if(minDistance > caster.GetViewRange())
            return null;

        return targetEntity;
    }

    private bool IsInRange(Vector2 sourcePos, Vector2 targetPos, float viewRange)
    {
        return (targetPos - sourcePos).magnitude < viewRange;
    }

    /// <summary>
    /// AddRange,封装是为了避免有条件要处理
    /// </summary>
    /// <param name="sourceList"></param>
    /// <param name="targetList"></param>
    private void InsertToTargetList(List<BattleUnit> sourceList, List<BattleUnit> targetList)
    {
        foreach(BattleUnit item in sourceList)
        {
            targetList.Add(item);
        }
    }

    private List<BattleUnit> FindTargetUnits(BattleUnit caster, MultipleTargetsTeam targetTeam, MultipleTargetsType targetDemageType)
    {
        List<BattleUnit> targets = new List<BattleUnit>(0);

        // 根据阵营找对象
        BattleCamp sourceCamp = caster.camp;
        if(targetTeam == MultipleTargetsTeam.UNIT_TARGET_TEAM_FRIENDLY)
        {
            targetTeam = sourceCamp == BattleCamp.FRIENDLY ? MultipleTargetsTeam.UNIT_TARGET_TEAM_FRIENDLY : MultipleTargetsTeam.UNIT_TARGET_TEAM_ENEMY;
        }
        else if(targetTeam == MultipleTargetsTeam.UNIT_TARGET_TEAM_ENEMY)  
        {
            targetTeam = sourceCamp == BattleCamp.FRIENDLY ? MultipleTargetsTeam.UNIT_TARGET_TEAM_ENEMY : MultipleTargetsTeam.UNIT_TARGET_TEAM_FRIENDLY;
        }

        switch(targetTeam)
        {
            case MultipleTargetsTeam.UNIT_TARGET_TEAM_ENEMY:
                InsertToTargetList(BattleEntityManager.instance.GetEntities(BattleCamp.ENEMY), targets);
                break;
            case MultipleTargetsTeam.UNIT_TARGET_TEAM_FRIENDLY:
                InsertToTargetList(BattleEntityManager.instance.GetEntities(BattleCamp.FRIENDLY), targets);
                break;
            case MultipleTargetsTeam.UNIT_TARGET_TEAM_BOTH:
                InsertToTargetList(BattleEntityManager.instance.GetEntities(BattleCamp.ENEMY), targets);
                InsertToTargetList(BattleEntityManager.instance.GetEntities(BattleCamp.FRIENDLY), targets);
                break;
            default:
                break;
        }
        return targets;
    }

    #region 搜索满足特定条件的一个敌人
    private Comparison<IComparable> MinValueCompare() { return (x, y) => x.CompareTo(y); }
    private Comparison<IComparable> MaxValueCompare() { return (x, y) => -x.CompareTo(y); }

    private BattleUnit GetRandomHeroEntity(List<BattleUnit> targetUnits)
    {
        int index = new System.Random().Next(0, targetUnits.Count - 1);
        return targetUnits[index];
    }

    private BattleUnit GetRangeHeroEntity(Vector2 sourcePos,List<BattleUnit> targetUnits,Comparison<IComparable> comparison)
    {
        targetUnits.Sort((x, y) =>
        {
            Vector2 distanceX = x.Get2DPosition() - sourcePos;
            Vector2 distanceY = y.Get2DPosition() - sourcePos;
            return comparison(distanceX.magnitude, distanceY.magnitude);
        });
        return targetUnits[0];
    }

    private BattleUnit GetHpHeroEntity(List<BattleUnit> targetUnits, Comparison<IComparable> comparison)
    {
        targetUnits.Sort((x, y) =>{return comparison(x.GetHP(), y.GetHP());});
        return targetUnits[0];
    }

    private BattleUnit GetHpPercentHeroEntity(List<BattleUnit> targetUnits, Comparison<IComparable> comparison)
    {
        targetUnits.Sort((x, y) => { return comparison(x.GetHPPercent(), y.GetHPPercent()); });
        return targetUnits[0];
    }

    private BattleUnit GetEnergyHeroEntity(List<BattleUnit> targetUnits, Comparison<IComparable> comparison)
    {
        targetUnits.Sort((x, y) => { return comparison(x.GetEnergy(), y.GetEnergy()); });
        return targetUnits[0];
    }

    private BattleUnit FilterTargetEntityFromList(BattleUnit caster, Ability ability, List<BattleUnit> targetUnits)
    {
        if(targetUnits.Count <= 0)
            return null;

        AbilityUnitAITargetCondition aiTargetCondition = ability.GetAiTargetCondition();
        switch(aiTargetCondition)
        {
            case AbilityUnitAITargetCondition.UNIT_TARGET_RANDOM:
                return GetRandomHeroEntity(targetUnits);
            case AbilityUnitAITargetCondition.UNIT_TARGET_RANGE_MIN:
                return GetRangeHeroEntity(caster.Get2DPosition(), targetUnits, MinValueCompare());
            case AbilityUnitAITargetCondition.UNIT_TARGET_RANGE_MAX:
                return GetRangeHeroEntity(caster.Get2DPosition(), targetUnits, MaxValueCompare());
            case AbilityUnitAITargetCondition.UNIT_TARGET_HP_MIN:
                return GetHpHeroEntity(targetUnits, MinValueCompare());
            case AbilityUnitAITargetCondition.UNIT_TARGET_HP_MAX:
                return GetHpHeroEntity(targetUnits, MaxValueCompare());
            case AbilityUnitAITargetCondition.UNIT_TARGET_HP_PERCENT_MIN:
                return GetHpPercentHeroEntity(targetUnits, MinValueCompare());
            case AbilityUnitAITargetCondition.UNIT_TARGET_HP_PERCENT_MAX:
                return GetHpPercentHeroEntity(targetUnits, MaxValueCompare());
            case AbilityUnitAITargetCondition.UNIT_TARGET_ENERGY_MIN:
                return GetEnergyHeroEntity(targetUnits, MinValueCompare());
            case AbilityUnitAITargetCondition.UNIT_TARGET_ENERGY_MAX:
                return GetEnergyHeroEntity(targetUnits, MaxValueCompare());
        }
        BattleLog.LogError("[FilterTargetUnitFromList]没有找到指定Entity,返回列表第一个单位！");
        return null;
    }

    #endregion

    public BattleUnit FindTargetUnitByAbility(BattleUnit caster, Ability ability)
    {
        // 全屏技能
        if(ability.GetCastRange() <= 0)
            return caster;

        BattleUnit lastTarget = caster.target;
        MultipleTargetsTeam targetTeam = ability.GetTargetTeam();
        MultipleTargetsType targetDemageType = ability.GetDamageType();

        BattleUnit newTarget;
        if(lastTarget == null || lastTarget.IsUnSelectable())
        {
            BattleCamp sourceCamp = caster.camp;
            List<BattleUnit> targets = FindTargetUnits(caster, targetTeam, targetDemageType);
            newTarget = FilterTargetEntityFromList(caster, ability, targets);
        }
        else
        {
            newTarget = lastTarget;
        }
        return newTarget;
    }

    public TargetCollection CalculateAOETarget(BattleUnit caster, AbilityData abilityData, RequestTarget requestTarget, ActionTarget actionTarget)
    {
        var targetCollection = new TargetCollection();

        // 根据中心类型获取目标点
        var centerType = actionTarget.Center;
        float centerX, centerZ;
        if(centerType == ActionMultipleTargetsCenter.CASTER)
        {
            caster.Get2DPosition(out centerX, out centerZ);
        }
        else if(centerType == ActionMultipleTargetsCenter.TARGET)
        {
            if(requestTarget.targetType == AbilityRequestTargetType.POINT)
            {
                BattleLog.LogError("单体技能[{0}]目标配置为TARGET，REQUEST TARGET却是POINT类型", abilityData.configFileName);
                return null;
            }
            requestTarget.GetTarget2DPosition(out centerX, out centerZ);
        }
        else if(centerType == ActionMultipleTargetsCenter.POINT)
        {
            requestTarget.GetTarget2DPosition(out centerX, out centerZ);
        }
        else
        {
            BattleLog.LogError("区域技能[{0}]中有未实现的技能中心点类型[{1}]", abilityData.configFileName, centerType);
            return null;
        }

        // 根据目标点和范围获取目标对象
        List<BattleUnit> tempTargets;
        var casterCamp = caster.camp;
        var Teams = actionTarget.Teams;
        var Types = actionTarget.Types;
        var aoeType = actionTarget.aoeType;
        if(aoeType == AOEType.Radius)
        {

        }
        else if(aoeType == AOEType.Line)
        {

        }
        else if(aoeType == AOEType.Sector)
        {

        }
        else
        {
            BattleLog.LogError("区域技能[{0}]中有未实现区域类型{1}", abilityData.configFileName, aoeType);
        }

        return targetCollection;
    }

    public TargetCollection CalculateSingleTarget(BattleUnit caster, AbilityData abilityData, RequestTarget requestTarget, ActionTarget actionTarget)
    {
        var targetCollection = new TargetCollection();
        if(actionTarget.singTarget == ActionSingTarget.CASTER)
        {
            targetCollection.targetType = AbilityRequestTargetType.UNIT;
            targetCollection.AddUnit(caster);
            return targetCollection;
        }

        if(requestTarget == null)
        {
            BattleLog.LogError("单体技能[{0}]的单体目标配置非CASTER，但是请求的目标为nil", abilityData.configFileName);
            return null;
        }

        if(actionTarget.singTarget == ActionSingTarget.TARGET)
        {
            if(requestTarget.targetType == AbilityRequestTargetType.POINT)
            {
                BattleLog.LogError("单体技能[{0}]目标配置为TARGET，REQUEST TARGET却是POINT类型", abilityData.configFileName);
                return null;
            }

            targetCollection.targetType = AbilityRequestTargetType.UNIT;
            targetCollection.AddUnit(requestTarget.GetTargetUnit());
            return targetCollection;
        }

        if(actionTarget.singTarget == ActionSingTarget.POINT)
        {
            targetCollection.targetType = AbilityRequestTargetType.POINT;
            targetCollection.AddPoint(requestTarget.targetPos);
            return targetCollection;
        }

        BattleLog.LogError("技能[{0}]中有未实现的目标类型[{1}]", abilityData.configFileName);
        return null;
    }

    /// <summary>
    /// 获取所有攻击目标
    /// </summary>
    public TargetCollection GetActionTargets(BattleUnit caster, AbilityData abilityData, RequestTarget requestTarget, ActionTarget actionTarget)
    {
        TargetCollection targetCollection;
        if(actionTarget.isSingleTarget)
            targetCollection = CalculateSingleTarget(caster, abilityData, requestTarget, actionTarget);
        else
            targetCollection = CalculateAOETarget(caster, abilityData, requestTarget, actionTarget);
     
        return targetCollection;
    }

    public List<BattleUnit> FindTargetUnitsByManualSelect(BattleUnit caster, Ability ability, 
        float dragWorldPointX = -1, float dragWorldPointZ = -1)
    {
        List<BattleUnit> targets = new List<BattleUnit>();
        var castRange = ability.GetCastRange();
        if(castRange <= 0)
        {
            targets.Add(caster);
            return targets;
        }

        // 单个敌人
        var abilityRange = ability.GetAbilityRange();
        if(abilityRange.isSingleTarget)
        {
            var unit = FindNearestEnemyUnit(caster);
            targets.Add(unit);
            return targets;
        }

        return null;
    }
}