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
    public BattleEntity FindNearestEnemyUnit(BattleEntity source)
    {
        BattleEntity targetEntity = null;
        // 找出敌对阵营
        BattleCamp enemyCamp = source.enemyCamp;
        // 获取指定标记的所有单位
        List<BattleEntity> entities = BattleEntityManager.instance.GetEntities(enemyCamp);
        // 比较距离，找出最近单位
        float minDistance = Int32.MaxValue;
        foreach(BattleEntity entity in entities)
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
    private void InsertToTargetList(BattleEntity source, List<BattleEntity> sourceList, List<BattleEntity> targetList)
    {
        foreach(BattleEntity item in sourceList)
        {
            if(IsInRange(source.Get2DPosition(),item.Get2DPosition(),source.GetViewRange()))
            {
                targetList.Add(item);
            }
        }
    }

    private List<BattleEntity> FindTargetUnits(BattleEntity source, AbilityUnitTargetTeam targetTeam, AbilityAreaDamageType targetDemageType)
    {
        List<BattleEntity> targets = new List<BattleEntity>(0);
        if(targetTeam == AbilityUnitTargetTeam.UNIT_TARGET_TEAM_NONE)
            return targets;

        // 根据阵营找对象
        BattleCamp sourceCamp = source.camp;
        if(targetTeam == AbilityUnitTargetTeam.UNIT_TARGET_TEAM_FRIENDLY)
        {
            targetTeam = sourceCamp == BattleCamp.FRIENDLY ? AbilityUnitTargetTeam.UNIT_TARGET_TEAM_FRIENDLY : AbilityUnitTargetTeam.UNIT_TARGET_TEAM_ENEMY;
        }
        else if(targetTeam == AbilityUnitTargetTeam.UNIT_TARGET_TEAM_ENEMY)  
        {
            targetTeam = sourceCamp == BattleCamp.FRIENDLY ? AbilityUnitTargetTeam.UNIT_TARGET_TEAM_ENEMY : AbilityUnitTargetTeam.UNIT_TARGET_TEAM_FRIENDLY;
        }

        switch(targetTeam)
        {
            case AbilityUnitTargetTeam.UNIT_TARGET_TEAM_ENEMY:
                InsertToTargetList(source, BattleEntityManager.instance.GetEntities(BattleCamp.ENEMY), targets);
                break;
            case AbilityUnitTargetTeam.UNIT_TARGET_TEAM_FRIENDLY:
                InsertToTargetList(source, BattleEntityManager.instance.GetEntities(BattleCamp.FRIENDLY), targets);
                break;
            case AbilityUnitTargetTeam.UNIT_TARGET_TEAM_BOTH:
                InsertToTargetList(source, BattleEntityManager.instance.GetEntities(BattleCamp.ENEMY), targets);
                InsertToTargetList(source, BattleEntityManager.instance.GetEntities(BattleCamp.FRIENDLY), targets);
                break;
            default:
                break;
        }
        return targets;
    }

    #region 搜索满足特定条件的一个敌人
    private Comparison<IComparable> MinValueCompare() { return (x, y) => x.CompareTo(y); }
    private Comparison<IComparable> MaxValueCompare() { return (x, y) => -x.CompareTo(y); }

    private BattleEntity GetRandomHeroEntity(List<BattleEntity> targetUnits)
    {
        int index = new System.Random().Next(0, targetUnits.Count - 1);
        return targetUnits[index];
    }

    private BattleEntity GetRangeHeroEntity(Vector2 sourcePos,List<BattleEntity> targetUnits,Comparison<IComparable> comparison)
    {
        targetUnits.Sort((x, y) =>
        {
            Vector2 distanceX = x.Get2DPosition() - sourcePos;
            Vector2 distanceY = y.Get2DPosition() - sourcePos;
            return comparison(distanceX.magnitude, distanceY.magnitude);
        });
        return targetUnits[0];
    }

    private BattleEntity GetHpHeroEntity(List<BattleEntity> targetUnits, Comparison<IComparable> comparison)
    {
        targetUnits.Sort((x, y) =>{return comparison(x.GetHP(), y.GetHP());});
        return targetUnits[0];
    }

    private BattleEntity GetHpPercentHeroEntity(List<BattleEntity> targetUnits, Comparison<IComparable> comparison)
    {
        targetUnits.Sort((x, y) => { return comparison(x.GetHPPercent(), y.GetHPPercent()); });
        return targetUnits[0];
    }

    private BattleEntity GetEnergyHeroEntity(List<BattleEntity> targetUnits, Comparison<IComparable> comparison)
    {
        targetUnits.Sort((x, y) => { return comparison(x.GetEnergy(), y.GetEnergy()); });
        return targetUnits[0];
    }

    private BattleEntity FilterTargetEntityFromList(BattleEntity source, Ability ability, List<BattleEntity> targetUnits)
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

    public BattleEntity FindTargetUnitByAbility(BattleEntity source,Ability ability)
    {
        //float castRange = ability.GetCastleRange();
        //float viewRange = source.GetViewRange();
        // 全屏技能
        if(ability.GetCastleRange() <= 0)
            return source;

        BattleEntity lastTagget = source.target;
        AbilityUnitTargetTeam targetTeam = ability.GetTargetTeam();
        AbilityAreaDamageType targetDemageType = ability.GetDamageType();

        BattleEntity newTarget;
        if(lastTagget == null || lastTagget.IsUnSelectable())
        {
            BattleCamp sourceCamp = source.camp;
            List<BattleEntity> targets = FindTargetUnits(source, targetTeam, targetDemageType);
            newTarget = FilterTargetEntityFromList(source, ability, targets);
        }
        else
        {
            newTarget = lastTagget;
        }
        return newTarget;
    }

    public List<BattleEntity> GetActionTarget(BattleEntity source, RequestTarget requestTarget)
    {
        List<BattleEntity> targets = new List<BattleEntity>();
        if(requestTarget.targetType == AbilityRequestTargetType.UNIT)
        {
            targets.Add(FindNearestEnemyUnit(source));
        }
        else
        {

        }
        return targets;
    }
}