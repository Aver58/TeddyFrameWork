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

public class NOD_CastAbility : BTAction
{
    protected override void OnEnter(BTData wData)
    {
        var behaviorData = wData as BattleData;
        BattleUnit source = behaviorData.owner;
        Ability ability = source.SelectCastableAbility();
        BattleLog.Log("【NOD_CastAbility】OnEnter {0}", ability.GetConfigName());
     
        source.CastAbility(ability);
    }

    protected override BTResult OnExecute(BTData wData)
    {
        var behaviorData = wData as BattleData;
        BattleUnit source = behaviorData.owner;
        Ability ability = source.SelectCastableAbility();

        //BattleLog.Log("【NOD_CastAbility】castTime：{0},duringTime：{1}", ability.castTime, ability.GetCastleDuring());
        if(ability.castTime >= ability.GetCastleDuring())
            return BTResult.Finished;

        return BTResult.Running;
    }
}