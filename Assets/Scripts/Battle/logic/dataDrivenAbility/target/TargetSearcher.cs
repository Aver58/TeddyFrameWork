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
    public BattleUnit FindNearestEnemyUnit(BattleUnit source)
    {
        BattleUnit targetEntity = null;
        // 找出敌对阵营
        BattleCamp enemyCamp = source.enemyCamp;
        // 获取指定标记的所有单位
        List<BattleUnit> entities = BattleEntityManager.instance.GetEntities(enemyCamp);
        // 比较距离，找出最近单位
        float minDistance = Int32.MaxValue;
        foreach(BattleUnit entity in entities)
        {
            Vector2 distance = source.Get2DPosition() - entity.Get2DPosition();
            if(distance.magnitude < minDistance)
            {
                targetEntity = entity;
                minDistance = distance.magnitude;
            }
        }

        if(minDistance > source.GetViewRange())
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

    private List<BattleUnit> FindTargetUnits(BattleUnit source, MultipleTargetsTeam targetTeam, MultipleTargetsType targetDemageType)
    {
        List<BattleUnit> targets = new List<BattleUnit>(0);

        // 根据阵营找对象
        BattleCamp sourceCamp = source.camp;
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

    private BattleUnit FilterTargetEntityFromList(BattleUnit source, Ability ability, List<BattleUnit> targetUnits)
    {
        if(targetUnits.Count <= 0)
            return null;

        AbilityUnitAITargetCondition aiTargetCondition = ability.GetAiTargetCondition();
        switch(aiTargetCondition)
        {
            case AbilityUnitAITargetCondition.UNIT_TARGET_RANDOM:
                return GetRandomHeroEntity(targetUnits);
            case AbilityUnitAITargetCondition.UNIT_TARGET_RANGE_MIN:
                return GetRangeHeroEntity(source.Get2DPosition(), targetUnits, MinValueCompare());
            case AbilityUnitAITargetCondition.UNIT_TARGET_RANGE_MAX:
                return GetRangeHeroEntity(source.Get2DPosition(), targetUnits, MaxValueCompare());
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

    public BattleUnit FindTargetUnitByAbility(BattleUnit source, Ability ability)
    {
        // 全屏技能
        if(ability.GetCastRange() <= 0)
            return source;

        BattleUnit lastTarget = source.target;
        MultipleTargetsTeam targetTeam = ability.GetTargetTeam();
        MultipleTargetsType targetDemageType = ability.GetDamageType();

        BattleUnit newTarget;
        if(lastTarget == null || lastTarget.IsUnSelectable())
        {
            BattleCamp sourceCamp = source.camp;
            List<BattleUnit> targets = FindTargetUnits(source, targetTeam, targetDemageType);
            newTarget = FilterTargetEntityFromList(source, ability, targets);
        }
        else
        {
            newTarget = lastTarget;
        }
        return newTarget;
    }

    public List<BattleUnit> GetActionTarget(BattleUnit source, RequestTarget requestTarget)
    {
        List<BattleUnit> targets = new List<BattleUnit>();
        if(requestTarget.targetType == AbilityRequestTargetType.UNIT)
        {
            targets.Add(FindNearestEnemyUnit(source));
        }
        else
        {

        }
        return targets;
    }

    public List<BattleUnit> FindTargetUnitsByManualSelect(BattleUnit source, Ability ability, 
        float dragWorldPointX = -1, float dragWorldPointZ = -1)
    {
        List<BattleUnit> targets = new List<BattleUnit>();
        var castRange = ability.GetCastRange();
        if(castRange <= 0)
        {
            targets.Add(source);
            return targets;
        }

        // 单个敌人
        var abilityRange = ability.GetAbilityRange();
        if(abilityRange.isSingleTarget)
        {
            var unit = FindNearestEnemyUnit(source);
            targets.Add(unit);
            return targets;
        }

        return null;
    }
}