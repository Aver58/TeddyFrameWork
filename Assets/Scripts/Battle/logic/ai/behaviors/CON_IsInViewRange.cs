using Aver3;
using UnityEngine;

public class CON_IsInViewRange : BTPrecondition
{
    public override bool IsTrue(BTData wData = null)
    {
        // 判断物体是否在视野范围内
        var data = wData as BattleData;
        BehaviorRequest request = data.request;
        BattleUnit owner = data.owner;
        float viewRange = owner.GetViewRange();
        Vector2 targetPos = request.target.Get2DPosition();
        Vector2 ownerPos = owner.Get2DPosition();
        float distanceSquare = (targetPos - ownerPos).magnitude;
        BattleLog.Log(string.Format("【CON_IsInViewRange】距离：{0}，可见范围：{1}", distanceSquare, viewRange * viewRange));
        return distanceSquare <= viewRange * viewRange;
    }
}