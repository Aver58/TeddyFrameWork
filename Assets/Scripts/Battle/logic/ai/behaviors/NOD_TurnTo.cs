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

using TsiU;
using UnityEngine;

public class NOD_TurnTo : TBTActionLeaf
{
    protected override void onEnter(TBTWorkingData wData){
        BattleLog.Log("【NOD_TurnTo】onEnter");
    }

    protected override void onExit(TBTWorkingData wData, int runningStatus){
        BattleLog.Log("【NOD_TurnTo】onExit");
    }

    protected override int onExecute(TBTWorkingData wData)
    {
        BattleBehaviorWorkingData behaviorData = wData as BattleBehaviorWorkingData;
        float deltaTime = behaviorData.deltaTime;
        BattleEntity source = behaviorData.owner;
        AIBehaviorRequest request = behaviorData.request;
        Entity target = request.target;

        Vector2 sourcePos = source.Get2DPosition();
        Vector2 targetPos = target.Get2DPosition();

        Vector2 sourceForward = source.Get2DForward();
        Vector2 targetForward = (targetPos - sourcePos).normalized;
        float turnSpeed = source.GetTurnSpeed();
        float angle = Vector2.Angle(targetForward, sourceForward);
        float radianToTurn = turnSpeed * deltaTime;

        if(angle <= radianToTurn * Mathf.Rad2Deg)
        {
            source.Set2DForward(targetForward);
            GameMsg.instance.SendMessage(GameMsgDef.Hero_TurnTo2D, new HeorTurnEventArgs()
            {
                id = source.id,
                forward = targetForward,
            });
            return TBTRunningStatus.FINISHED;
        }

        // 叉积算出方向 unity是左手坐标系，所以反过来了
        Vector3 cross = Vector3.Cross(targetForward, sourceForward);
        if(cross.z > 0)
        {
            radianToTurn = -radianToTurn;
        }
        Vector2 newForward = BattleMath.Vector2RotateFromRadian(sourceForward.x, sourceForward.y,radianToTurn);

        //BattleLog.Log("【NOD_TurnTo】自己朝向：{0} 目标朝向：{1} 相隔角度：{2} 旋转弧度：{3} 叉乘：{4} 新的朝向：{5}", sourceForward.ToString(),targetForward.ToString(), angle.ToString(),(radianToTurn * Mathf.Rad2Deg).ToString(), cross.ToString(), newForward.ToString());
        source.Set2DForward(newForward);
        GameMsg.instance.SendMessage(GameMsgDef.Hero_TurnTo2D, new HeorTurnEventArgs()
        {
            id = source.id,
            forward = newForward,
        });
        return TBTRunningStatus.EXECUTING;
    }
}