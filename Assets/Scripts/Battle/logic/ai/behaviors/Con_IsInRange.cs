using UnityEngine;
using TsiU;

public class CON_IsInRange : TBTPrecondition
{
    public override bool IsTrue(TBTWorkingData wData = null)
    {
        // 判断物体是否在视野范围内
        BattleBehaviorWorkingData data = wData as BattleBehaviorWorkingData;
        AIBehaviorRequest request = data.request;
        BattleEntity owner = data.owner;
        float viewRange = owner.GetViewRange();
        Vector2 targetPos = request.target.Get2DPosition();
        Vector2 ownerPos = owner.Get2DPosition();
        float distanceSquare = (targetPos - ownerPos).magnitude;
        BattleLog.Log(string.Format("【CON_IsInRange】距离：{0}，可见范围：{1}", distanceSquare, viewRange * viewRange));
        return distanceSquare <= viewRange * viewRange;
    }
}