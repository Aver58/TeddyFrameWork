#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    CON_IsChaseRequest.cs
 Author:      Zeng Zhiwei
 Time:        2020\5\27 星期三 0:00:56
=====================================================
*/
#endregion

using TsiU;

public class CON_IsManualCastAbilityRequest : TBTPreconditionLeaf
{
    public override bool IsTrue(TBTWorkingData wData)
    {
        BattleBehaviorWorkingData data = wData as BattleBehaviorWorkingData;
        if(data.request != null && data.request.RequestType == RequestType.ManualCastAbility)
            return true;
        return false;
    }
}