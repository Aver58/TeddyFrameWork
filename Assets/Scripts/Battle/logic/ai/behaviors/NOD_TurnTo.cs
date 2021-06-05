#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    NOD_TurnTo.cs
 Author:      Zeng Zhiwei
 Time:        2020/8/18 19:25:05
=====================================================
*/
#endregion

using Aver3;
using UnityEngine;

public class NOD_TurnTo : BTAction
{
    protected override void OnEnter(BTData bTData)
    {
        var behaviorData = bTData as BattleData;
        BattleUnit owner = behaviorData.owner;
        owner.SetState(HeroState.TURN);
    }

    protected override BTResult OnExecute(BTData bTData)
    {
        var behaviorData = bTData as BattleData;
        float deltaTime = behaviorData.deltaTime;
        BattleUnit source = behaviorData.owner;
        BehaviorRequest request = behaviorData.request;
        Unit target = request.target;

        var ownerPos = source.Get3DPosition();
        var targetPos = target.Get3DPosition();

        var sourceForward = source.Get3DForward();
        var targetForward = (targetPos - ownerPos).normalized;
        float turnSpeed = source.GetTurnSpeed();
        float angle = Vector3.Angle(targetForward, sourceForward);
        float radianToTurn = turnSpeed * deltaTime;

        BattleLog.Log("【NOD_TurnTo】angle:{0},{1}", angle, radianToTurn * Mathf.Rad2Deg);
        if(angle < radianToTurn * Mathf.Rad2Deg)
        {
            source.Set3DForward(targetForward);
            GameMsg.instance.SendMessage(GameMsgDef.Hero_TurnTo3D, source.id, targetForward.x, targetForward.y, targetForward.z);
            return BTResult.Finished;
        }

        // 叉积算出方向 unity是左手坐标系，所以反过来了
        Vector3 cross = Vector3.Cross(targetForward, sourceForward);
        if(cross.z > 0)
            radianToTurn = -radianToTurn;

        float y = targetForward.y;
        BattleMath.RotateByYAxis(sourceForward.x, sourceForward.y, sourceForward.z,radianToTurn,out float x,ref y,out float z);
        source.Set3DForward(x,y,z);
        GameMsg.instance.SendMessage(GameMsgDef.Hero_TurnTo3D , source.id, x,y,z);
        GameLog.Log("【NOD_TurnTo】自己朝向：{0} 目标朝向：{1} 相隔角度：{2} 旋转弧度：{3} 叉乘：{4} 新的朝向：{5},{6},{7}", sourceForward, targetForward, angle, radianToTurn * Mathf.Rad2Deg, cross, x, y, z);
        return BTResult.Running;
    }
}