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
    private float m_TargetPointX;
    private float m_TargetPointZ;
    public AbilityInputDirection(Ability ability, HeroActor casterActor) : base(ability, casterActor)
    {
    }

    public override void OnFingerDrag(float casterX, float casterZ, float dragWorldPointX, float dragWorldPointZ, float dragForwardX, float dragForwardZ)
    {
        base.OnFingerDrag(casterX, casterZ, dragWorldPointX, dragWorldPointZ, dragForwardX, dragForwardZ);
        m_TargetPointX = dragForwardX;
        m_TargetPointZ = dragForwardZ;
    }

    protected override void OnManualCastSkill()
    {
        if(m_Ability.CD > 0)
        {
            GameLog.Log(m_Ability.GetConfigName() + "冷却中...");
            return;
        }

        // 向决策树输入请求
        m_Ability.SetPointTarget(m_TargetPointX, m_TargetPointZ);
        var request = new ManualCastAbilityRequest(m_Ability);
        m_CasterActor.ForceUpdateDecisionRequest(request);
    }
}