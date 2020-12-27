﻿#region Copyright © 2020 Aver. All rights reserved.
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

public class D2Action
{
    private List<BattleEntity> m_Targets;
    protected RequestTarget m_RequestTarget;
    protected AbilityData m_AbilityData;

    public D2Action(AbilityData abilityData)
    {
        m_AbilityData = abilityData;
    }

    public virtual void Execute(BattleEntity source, RequestTarget requestTarget)
    {
        BattleLog.Log("【D2Action】{0}， source：{1}，target：{2}" ,GetType().Name, source.GetName(), requestTarget.ToString());

        m_RequestTarget = requestTarget;
        m_Targets = TargetSearcher.instance.GetActionTarget(source, requestTarget);
        // 根据类型区分，单位和范围
        if(requestTarget.targetType == AbilityRequestTargetType.UNIT)
        {
            BattleEntity target = m_Targets[0];
            ExecuteByUnit(source, target);
        }
        else
        {
            ExecuteByPoint(source, m_Targets);
        }
    }

    /// <summary>
    /// 单体技能
    /// </summary>
    protected virtual void ExecuteByUnit(BattleEntity source, BattleEntity target) { }
    /// <summary>
    /// 范围伤害
    /// </summary>
    protected virtual void ExecuteByPoint(BattleEntity source, List<BattleEntity> targets) { }
}