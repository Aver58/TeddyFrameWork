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

public class CON_IsChaseRequest : TBTPreconditionLeaf
{
    public override bool IsTrue(TBTWorkingData wData)
    {
        BattleBehaviorWorkingData data = wData as BattleBehaviorWorkingData;
        //BattleLog.Log("【CON_IsChaseRequest】data：{0}", data.ToString());
        if(data.request !=null && data.request.RequestType == RequestType.Chase)
            return true;
        return false;
    }
}