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

        Vector2 sourcePos = source.Get2DPosition();
        Vector2 targetPos = target.Get2DPosition();

        Vector2 sourceForward = source.Get2DForward();
        Vector2 targetForward = (targetPos - sourcePos).normalized;
        float turnSpeed = source.GetTurnSpeed();
        float angle = Vector2.Angle(targetForward, sourceForward);
        float radianToTurn = turnSpeed * deltaTime;

        BattleLog.Log("【NOD_TurnTo】angle:{0},{1}", angle, radianToTurn * Mathf.Rad2Deg);
        if(angle < radianToTurn * Mathf.Rad2Deg)
        {
            source.Set2DForward(targetForward);
            GameMsg.instance.SendMessage(GameMsgDef.Hero_TurnTo2D, source.id, targetForward);
            return BTResult.Finished;
        }

        // 叉积算出方向 unity是左手坐标系，所以反过来了
        Vector3 cross = Vector3.Cross(targetForward, sourceForward);
        if(cross.z > 0)
            radianToTurn = -radianToTurn;

        Vector2 newForward = BattleMath.Vector2RotateFromRadian(sourceForward.x, sourceForward.y,radianToTurn);
        source.Set2DForward(newForward);
        GameMsg.instance.SendMessage(GameMsgDef.Hero_TurnTo2D , source.id, targetForward);
        GameLog.Log("【NOD_TurnTo】自己朝向：{0} 目标朝向：{1} 相隔角度：{2} 旋转弧度：{3} 叉乘：{4} 新的朝向：{5}", sourceForward.ToString(), targetForward.ToString(), angle.ToString(), (radianToTurn * Mathf.Rad2Deg).ToString(), cross.ToString(), newForward.ToString());
        return BTResult.Running;
    }
}