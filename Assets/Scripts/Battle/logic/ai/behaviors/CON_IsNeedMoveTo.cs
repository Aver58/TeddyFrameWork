#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    CON_IsArrival.cs
 Author:      Zeng Zhiwei
 Time:        2021/6/2 16:23:00
=====================================================
*/
#endregion

using Aver3;
using UnityEngine;

public class CON_IsNeedMoveTo : BTPrecondition
{
    public override bool IsTrue(BTData bTData)
    {
        var data = bTData as BattleData;
        var request = data.request as ChaseRequest;
        var target = request.target;
        var owner = data.owner;
        var ownerPos = owner.Get2DPosition();
        var targetPos = target.Get2DPosition();

        var distance = Vector2.Distance(targetPos , ownerPos);
        return distance > 0;
    }
}