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
        List<BattleUnit> entities = BattleUnitManager.Instance.GetEntities(enemyCamp);
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

    /// <summary>
    /// 用配置的ai筛选条件筛选目标
    /// </summary>
    private BattleUnit FilterTargetUnitFromList(BattleUnit caster, Ability ability, List<BattleUnit> targetUnits)
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
            default:
                return null;
        }
    }

    #endregion

    public TargetCollection CalculateAOETarget(BattleUnit caster, AbilityData abilityData, RequestTarget requestTarget, ActionTarget actionTarget)
    {
        var targetCollection = new TargetCollection();

        // 根据中心类型获取目标点
        var centerType = actionTarget.Target;
        float centerX, centerZ;
        if(centerType == ActionMultipleTargets.CASTER)
        {
            caster.Get2DPosition(out centerX, out centerZ);
        }
        else if(centerType == ActionMultipleTargets.TARGET)
        {
            if(requestTarget.targetType == AbilityRequestTargetType.POINT)
            {
                BattleLog.LogError("单体技能[{0}]目标配置为TARGET，REQUEST TARGET却是POINT类型", abilityData.configFileName);
                return null;
            }
            requestTarget.GetTarget2DPosition(out centerX, out centerZ);
        }
        else if(centerType == ActionMultipleTargets.POINT)
        {
            requestTarget.GetTarget2DPosition(out centerX, out centerZ);
        }
        else
        {
            BattleLog.LogError("区域技能[{0}]中有未实现的技能中心点类型[{1}]", abilityData.configFileName, centerType);
            return null;
        }

        // 根据目标点和范围获取目标对象
        var casterCamp = caster.camp;
        var Teams = actionTarget.targetTeam;
        var Types = actionTarget.targetType;
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
        if(actionTarget.SingleTarget == ActionSingleTarget.CASTER)
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

        if(actionTarget.SingleTarget == ActionSingleTarget.TARGET)
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

        if(actionTarget.SingleTarget == ActionSingleTarget.POINT)
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

    private List<BattleUnit> FindTargetUnits(BattleCamp sourceCamp, AbilityUnitTargetTeam targetTeam, AbilityUnitTargetType targetTypes,AbilityUnitTargetFlag targetFlag)
    {
        List<BattleUnit> targets = new List<BattleUnit>(0);

        // 根据阵营找对象
        if(targetTeam == AbilityUnitTargetTeam.DOTA_UNIT_TARGET_TEAM_FRIENDLY)
        {
            targetTeam = sourceCamp == BattleCamp.FRIENDLY ? AbilityUnitTargetTeam.DOTA_UNIT_TARGET_TEAM_FRIENDLY : AbilityUnitTargetTeam.DOTA_UNIT_TARGET_TEAM_ENEMY;
        }
        else if(targetTeam == AbilityUnitTargetTeam.DOTA_UNIT_TARGET_TEAM_ENEMY)
        {
            targetTeam = sourceCamp == BattleCamp.FRIENDLY ? AbilityUnitTargetTeam.DOTA_UNIT_TARGET_TEAM_ENEMY : AbilityUnitTargetTeam.DOTA_UNIT_TARGET_TEAM_FRIENDLY;
        }

        switch(targetTeam)
        {
            case AbilityUnitTargetTeam.DOTA_UNIT_TARGET_TEAM_ENEMY:
                InsertToTargetList(BattleUnitManager.Instance.GetEntities(BattleCamp.ENEMY), targets);
                break;
            case AbilityUnitTargetTeam.DOTA_UNIT_TARGET_TEAM_FRIENDLY:
                InsertToTargetList(BattleUnitManager.Instance.GetEntities(BattleCamp.FRIENDLY), targets);
                break;
            case AbilityUnitTargetTeam.DOTA_UNIT_TARGET_TEAM_BOTH:
                InsertToTargetList(BattleUnitManager.Instance.GetEntities(BattleCamp.ENEMY), targets);
                InsertToTargetList(BattleUnitManager.Instance.GetEntities(BattleCamp.FRIENDLY), targets);
                break;
            default:
                break;
        }
        return targets;
    }

    /// <summary>
    /// 寻找技能范围内所有目标
    /// </summary>
    public List<BattleUnit> FindTargetUnitsByAbilityRange(BattleUnit caster, Ability ability)
    {
        List<BattleUnit> targets = new List<BattleUnit>();
        // 全屏技能
        var radius = ability.GetCastRange();
        if(radius <= 0)
        {
            targets.Add(caster);
            return targets;
        }

        caster.Get2DPosition(out float centerX, out float centerZ);
        var targetTeams = ability.GetTargetTeam();
        var targetTypes = ability.GetTargetType();
        var tempTargets = FindTargetUnitsInRadius(caster.camp,targetTeams,targetTypes,default,centerX,centerZ,radius);
        return tempTargets;
    }

    public BattleUnit FindTargetUnitByAbilityRange(BattleUnit caster, Ability ability)
    {
        var tempTargets = FindTargetUnitsByAbilityRange(caster, ability);
        return FilterTargetUnitFromList(caster, ability, tempTargets);
    }

    /// <summary>
    /// 获取在指定半径范围内的单位
    /// </summary>
    public List<BattleUnit> FindTargetUnitsInRadius(BattleCamp battleCamp, AbilityUnitTargetTeam targetTeams,AbilityUnitTargetType targetTypes,AbilityUnitTargetFlag targetFlag,float centerX, float centerZ, float radius)
    {
        var targetUnits = FindTargetUnits(battleCamp, targetTeams, targetTypes, targetFlag);

        for(int i = targetUnits.Count - 1; i > 0 ; i--)
        {
            var unit = targetUnits[i];
            unit.Get2DPosition(out float posX, out float posZ);
            if(!BattleMath.IsPointInCircle(posX,posZ,centerX,centerZ,radius))
                targetUnits.RemoveAt(i);
        }
        return targetUnits;
    }
}