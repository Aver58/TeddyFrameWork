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

using TsiU;
using UnityEngine;

public class CON_IsAngleNeedTurnTo : TBTPreconditionLeaf
{
    public override bool IsTrue(TBTWorkingData wData)
    {
        BattleBehaviorWorkingData behaviorData = wData as BattleBehaviorWorkingData;
        BattleUnit owner = behaviorData.owner;
        AIBehaviorRequest request = behaviorData.request;
        Unit target = request.target;

        Vector2 ownerPos = owner.Get2DPosition();
        Vector2 targetPos = target.Get2DPosition();

        Vector2 toForward = targetPos - ownerPos;
        Vector2 ownerForward = owner.Get2DForward();
        float angle = Vector2.Angle(toForward, ownerForward);
        //BattleLog.Log("【CON_IsAngleNeedTurnTo】angle:{0}", angle);
        return angle > 0.1f;
    }
}