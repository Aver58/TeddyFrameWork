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


using Aver3;

public class NOD_SetUnitIdle : BTAction
{
    protected override void OnEnter(BTData wData)
    {
        BattleLog.Log("【NOD_SetUnitIdle】onEnter");
        var behaviorData = wData as BattleData;
        BattleUnit source = behaviorData.owner;
        var request = behaviorData.request;
        
        source.SetState(HeroState.IDLE);
        request.isRequestCompleted = true;
    }
}