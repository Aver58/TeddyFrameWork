#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    CON_IsAngleNeedTurnTo.cs
 Author:      Zeng Zhiwei
 Time:        2020/8/18 19:33:22
=====================================================
*/
#endregion

using Aver3;
using UnityEngine;

public class CON_IsNeedTurnTo : BTPrecondition
{
    public override bool IsTrue(BTData bTData)
    {
        BattleData behaviorData = bTData as BattleData;
        BattleUnit owner = behaviorData.owner;
        BehaviorRequest request = behaviorData.request;
        Unit target = request.target;

        var ownerPos = owner.Get3DPosition();
        var targetPos = target.Get3DPosition();

        var toForward = (targetPos - ownerPos).normalized;
        var ownerForward = owner.Get3DForward();

        float angle = Vector3.Angle(toForward, ownerForward);
        BattleLog.Log("【{0}】angle:{1},toForward:{2},ownerForward:{3},targetPos:{4},ownerPos,{5}", GetType().ToString(), angle, toForward, ownerForward, targetPos, ownerPos);
        return angle > 0.01f;
    }
}