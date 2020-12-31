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

using TsiU;

public class NOD_CastAbility : TBTActionLeaf
{
    protected override void onEnter(TBTWorkingData wData)
    {
        BattleBehaviorWorkingData behaviorData = wData as BattleBehaviorWorkingData;
        BattleEntity source = behaviorData.owner;
        Ability ability = source.SelectCastableAbility();
        BattleLog.Log("【NOD_CastAbility】onEnter {0}", ability.GetConfigName());
     
        source.CastAbility(ability);
    }

    protected override void onExit(TBTWorkingData wData, int runningStatus)
    {
        BattleLog.Log("【NOD_CastAbility】onExit");
    }

    protected override int onExecute(TBTWorkingData wData)
    {
        BattleBehaviorWorkingData behaviorData = wData as BattleBehaviorWorkingData;
        BattleEntity source = behaviorData.owner;
        Ability ability = source.SelectCastableAbility();

        //BattleLog.Log("【NOD_CastAbility】castTime：{0},duringTime：{1}", ability.castTime, ability.GetCastleDuring());
        if(ability.castTime >= ability.GetCastleDuring())
            return TBTRunningStatus.FINISHED;

        return TBTRunningStatus.EXECUTING;
    }
}