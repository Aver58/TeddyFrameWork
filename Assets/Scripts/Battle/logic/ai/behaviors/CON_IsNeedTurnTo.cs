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

        Vector2 ownerPos = owner.Get2DPosition();
        Vector2 targetPos = target.Get2DPosition();

        Vector2 toForward = targetPos - ownerPos;
        Vector2 ownerForward = owner.Get2DForward();

        float angle = Vector2.Angle(toForward, ownerForward);
        BattleLog.Log("【{0}】angle:{1}", GetType().ToString(), angle);
        return angle > 0.1f;
    }
}