#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    CON_CanUpdateRequest.cs
 Author:      Zeng Zhiwei
 Time:        2020\5\18 星期一 23:48:06
=====================================================
*/
#endregion

using TsiU;

public class CON_CanUpdateRequest : TBTPrecondition
{
    public override bool IsTrue(TBTWorkingData wData)
    {
        BattleDecisionWorkingData decisionData = wData as BattleDecisionWorkingData;
        BattleUnit battleEntity = decisionData.owner;
        if(!battleEntity.IsCanDecision())
            return false;
        
        // 新的决策
        BehaviorRequest request = decisionData.request;
        if(request == null)
            return true;
        else
        {
            //上一个请求已经执行完成，就更新新请求 or 追逐请求中，也可以更新请求替换它(比如技能可以使用了等情况)
            var isRequestCompleted = request.IsRequestCompleted();
            var isChaseRequest = request.RequestType == RequestType.Chase;
            return isRequestCompleted || isChaseRequest;
        }
    }
}