#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    AbilityInputTarget.cs
 Author:      Zeng Zhiwei
 Time:        2021/1/14 14:42:07
=====================================================
*/
#endregion

using UnityEngine;

public class AbilityInputTarget : AbilityInput
{
    private BattleUnit m_target; 
    public AbilityInputTarget(Transform casterTransform, Ability ability) : base(casterTransform, ability){}

    public override void OnFingerDrag(float casterX, float casterZ, float dragWorldPointX, float dragWorldPointZ, float dragForwardX, float dragForwardZ)
    {
        base.OnFingerDrag(casterX, casterZ, dragWorldPointX, dragWorldPointZ, dragForwardX, dragForwardZ);

        var entitys = TargetSearcher.instance.FindTargetUnitsByManualSelect(m_ability.caster,m_ability);
        if(entitys.Count != 1)
            return;

        m_target = entitys[0];
    }

    protected override void OnManualCastSkill()
    {
        if(m_ability.CD > 0)
        {
            GameLog.Log(m_ability.GetConfigName() + "冷却中...");
            return;
        }

        // 向决策树输入请求
        var request = new ManualCastAbilityRequest(m_ability,m_target);
        var playerUnit = BattleUnitManager.instance.playerUnit;
        playerUnit.ForceUpdateDecisionRequest(request);
    }
}