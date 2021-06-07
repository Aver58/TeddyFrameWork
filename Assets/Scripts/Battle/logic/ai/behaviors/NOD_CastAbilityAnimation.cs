#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    NOD_CastAbility.cs
 Author:      Zeng Zhiwei
 Time:        2020/8/27 9:53:35
=====================================================
*/
#endregion


using Aver3;

public class NOD_CastAbilityAnimation : BTAction
{
    protected override void OnEnter(BTData wData)
    {
        var behaviorData = wData as BattleData;
        BattleUnit source = behaviorData.owner;
        var request = behaviorData.request as ManualCastAbilityRequest;
        Ability ability = request.ability;
        BattleLog.Log("【NOD_CastAbilityAnimation】OnEnter {0}", ability.GetConfigName());
     
        source.CastAbilityAnimation(ability);
    }
}