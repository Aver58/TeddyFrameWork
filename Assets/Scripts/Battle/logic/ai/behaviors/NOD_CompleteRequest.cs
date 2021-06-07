#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    NOD_CompleteRequest.cs
 Author:      Zeng Zhiwei
 Time:        2021/6/7 11:31:45
=====================================================
*/
#endregion

using Aver3;

public class NOD_CompleteRequest : BTAction
{
    protected override void OnEnter(BTData bTData)
    {
        var behaviorData = bTData as BattleData;
        behaviorData.owner.SetState(HeroState.IDLE);
        behaviorData.owner.SetRequestComplete();
    }
}