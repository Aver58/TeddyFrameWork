#region Copyright © 2018 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    Nod_Move.cs
 Author:      Zeng Zhiwei
 Time:        2020/3/4 20:01:27
=====================================================
*/
#endregion

using UnityEngine;

namespace Aver3
{
    public class NOD_MoveTo : BTAction
    {
        protected override void OnEnter(BTData wData)
        {
            GameLog.Log("NOD_MoveTo OnEnter");
            var behaviorData = wData as BattleData;
            BattleUnit owner = behaviorData.owner;
            owner.SetState(HeroState.MOVE);
        }

        protected override void OnExit(BTData bTData)
        {
            GameLog.Log("NOD_MoveTo OnExit");
            var behaviorData = bTData as BattleData;
            behaviorData.owner.SetRequestComplete();
        }

        protected override BTResult OnExecute(BTData wData)
        {
            var behaviorData = wData as BattleData;
            var owner = behaviorData.owner;
            var request = behaviorData.request;
            var target = request.target;

            var ownerPos = owner.Get3DPosition();
            var targetPos = target.Get3DPosition();

            float distance = (targetPos - ownerPos).magnitude;

            if(distance <= 0.01f)
                return BTResult.Finished;

            // 旋转
            var sourceForward = owner.Get3DForward();
            var targetForward = (targetPos - ownerPos).normalized;
            float turnSpeed = owner.GetTurnSpeed();
            float angle = Vector3.Angle(targetForward, sourceForward);
            float radianToTurn = turnSpeed * behaviorData.deltaTime;
            BattleLog.Log("【NOD_TurnTo】angle:{0},{1}", angle, radianToTurn * Mathf.Rad2Deg);
            if(angle < radianToTurn * Mathf.Rad2Deg)
            {
                owner.Set3DForward(targetForward);
                GameMsg.instance.SendMessage(GameMsgDef.Hero_TurnTo3D, owner.id, targetForward.x, targetForward.y, targetForward.z);
            }
            else
            {
                // 叉积算出方向 unity是左手坐标系，所以反过来了
                Vector3 cross = Vector3.Cross(targetForward, sourceForward);
                if(cross.z > 0)
                    radianToTurn = -radianToTurn;

                float y = targetForward.y;
                BattleMath.RotateByYAxis(sourceForward.x, sourceForward.y, sourceForward.z, radianToTurn, out float x, ref y, out float z);
                owner.Set3DForward(x, y, z);
                GameMsg.instance.SendMessage(GameMsgDef.Hero_TurnTo3D, owner.id, x, y, z);
                GameLog.Log("【NOD_TurnTo】自己朝向：{0} 目标朝向：{1} 相隔角度：{2} 旋转弧度：{3} 叉乘：{4} 新的朝向：{5},{6},{7}", sourceForward, targetForward, angle, radianToTurn * Mathf.Rad2Deg, cross, x, y, z);
            }

            // 移动
            float moveSpeed = owner.GetMoveSpeed();
            float detalTime = behaviorData.deltaTime;
            //float toForwardX = (targetPos.x - ownerPos.x) / distance;
            //float toForwardZ = (targetPos.z - ownerPos.z) / distance;
            float newPosX = ownerPos.x + detalTime * moveSpeed * targetForward.x;
            float newPosZ = ownerPos.z + detalTime * moveSpeed * targetForward.z;

            if(Mathf.Abs(newPosX) > Mathf.Abs(targetPos.x) || Mathf.Abs(newPosZ) > Mathf.Abs(targetPos.z))
            {
                owner.Set3DPosition(targetPos);
                GameMsg.instance.SendMessage(GameMsgDef.Hero_MoveTo, owner.id, targetPos.x, targetPos.y, targetPos.z);
            }
            else
            {
                owner.Set3DPosition(newPosX, targetPos.y, newPosZ);
                GameMsg.instance.SendMessage(GameMsgDef.Hero_MoveTo, owner.id, newPosX, targetPos.y, newPosZ);
            }
          
            GameLog.Log("【NOD_MoveTo】移动速度：{0} 当前位置：{1},{2} 目标位置：{3},{4} newPosX：{5} newPosZ：{6}",moveSpeed, ownerPos.x, ownerPos.y, targetPos.x, targetPos.y, newPosX, newPosZ);
            return BTResult.Running;
        }
    }
}