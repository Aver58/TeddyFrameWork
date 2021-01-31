#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    NOD_SetUnitIdle.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/29 14:57:25
=====================================================
*/
#endregion

using TsiU;

public class NOD_SetUnitIdle : TBTActionLeaf
{
    protected override void onEnter(TBTWorkingData wData)
    {
        BattleLog.Log("【NOD_SetUnitIdle】onEnter");
        BattleBehaviorWorkingData behaviorData = wData as BattleBehaviorWorkingData;
        BattleUnit source = behaviorData.owner;
        var request = behaviorData.request;
        
        source.SetState(HeroState.IDLE);
        request.isRequestCompleted = true;
    }
}