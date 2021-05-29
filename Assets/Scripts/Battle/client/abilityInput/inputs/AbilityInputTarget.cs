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

using System.Collections.Generic;
using UnityEngine;

public class AbilityInputTarget : AbilityInput
{
    private List<HeroActor> m_targets; 
    public AbilityInputTarget(Transform casterTransform, Ability ability) : base(casterTransform, ability)
    {
    }

    public override void OnFingerDrag(float casterX, float casterZ, float dragWorldPointX, float dragWorldPointZ, float dragForwardX, float dragForwardZ)
    {
        base.OnFingerDrag(casterX, casterZ, dragWorldPointX, dragWorldPointZ, dragForwardX, dragForwardZ);

        var entitys = TargetSearcher.instance.FindTargetUnitsByManualSelect(m_ability.caster,m_ability);
        if(entitys.Count != 1)
            return;
        
        var actors = BattleActorManager.instance.GetActors(entitys);
        var target = actors[0];
        // 更新角色朝向
        target.Set2DForward(dragForwardX, dragForwardZ);
    }

    protected new void OnManualCastSkill()
    {

    }
}