using TsiU;
using UnityEngine;

public class CON_IsInAbilityRange : TBTPrecondition
{
    public override bool IsTrue(TBTWorkingData wData = null)
    {
        // 判断物体是否在视野范围内
        BattleBehaviorWorkingData data = wData as BattleBehaviorWorkingData;
        AutoCastAbilityRequest request = data.request as AutoCastAbilityRequest;
        BattleEntity owner = data.owner;
        Entity target = request.target;
        // 目标是自己
        if(target == owner)
            return true;

        Ability ability = request.ability;
        float abilityRange = ability.GetCastRange();
        // 全屏技能
        if(abilityRange == -1)
            return true;
        
        Vector2 ownerPos = owner.Get2DPosition();
        Vector2 targetPos = target.Get2DPosition();
        float distance = (targetPos - ownerPos).magnitude;
        //BattleLog.Log(string.Format("【CON_IsInAbilityRange】距离：{0}，技能范围：{1}", distance, abilityRange));
        return distance <= abilityRange;
    }
}