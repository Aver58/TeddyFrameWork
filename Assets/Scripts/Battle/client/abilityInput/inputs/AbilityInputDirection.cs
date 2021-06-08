#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    AbilityInputDirection.cs
 Author:      Zeng Zhiwei
 Time:        2021/1/13 15:13:56
=====================================================
*/
#endregion

using UnityEngine;

public class AbilityInputDirection : AbilityInput
{
    public AbilityInputDirection(Transform casterTransform, Ability m_ability) : base(casterTransform, m_ability)
    {

    }

    protected override void OnManualCastSkill()
    {
        if(m_ability.CD > 0)
        {
            GameLog.Log(m_ability.GetConfigName() + "冷却中...");
            return;
        }

        // 向决策树输入请求
        //var request = new ManualCastAbilityRequest(m_ability, m_target);
        //var playerUnit = BattleUnitManager.instance.playerUnit;
        //playerUnit.ForceUpdateDecisionRequest(request);
    }
}