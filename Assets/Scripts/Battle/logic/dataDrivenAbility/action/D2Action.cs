#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    D2Action.cs
 Author:      Zeng Zhiwei
 Time:        2020/6/20 14:03:45
=====================================================
*/
#endregion

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 技能行为
/// Each of these keys is a block that contains one or more action descriptions.
/// </summary>
public class D2Action
{
    private List<BattleUnit> m_targets;

    protected AbilityData abilityData;
    protected ActionTarget actionTarget;
    protected RequestTarget requestTarget;

    public D2Action(ActionTarget actionTarget)
    {
        this.actionTarget = actionTarget;
    }

    public virtual void Execute(BattleUnit source, AbilityData abilityData, RequestTarget requestTarget)
    {
        this.abilityData = abilityData;
        this.requestTarget = requestTarget;

        var targetCollection = TargetSearcher.instance.GetActionTargets(source, abilityData, requestTarget, actionTarget);
        BattleLog.Log("【D2Action】{0}， source：{1}，target：{2}" ,GetType().Name, source.GetName(), requestTarget.ToString());
        // 根据类型区分，单位和范围
        if(requestTarget.targetType == AbilityRequestTargetType.UNIT)
        {
            ExecuteByUnit(source, targetCollection.units);
        }
        else
        {
            ExecuteByPoint(source, targetCollection.points);
        }
    }

    /// <summary>
    /// 单体技能
    /// </summary>
    protected virtual void ExecuteByUnit(BattleUnit source, List<BattleUnit> targets) { }
    /// <summary>
    /// 范围伤害
    /// </summary>
    protected virtual void ExecuteByPoint(BattleUnit source, List<Vector2> targets) { }
}