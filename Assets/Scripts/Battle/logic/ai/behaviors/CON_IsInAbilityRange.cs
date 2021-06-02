using Aver3;
using UnityEngine;

public class CON_IsInAbilityRange : BTPrecondition
{
    public override bool IsTrue(BTData wData)
    {
        // 判断物体是否在视野范围内
        var data = wData as BattleData;
        var request = data.request as AutoCastAbilityRequest;
        BattleUnit owner = data.owner;
        Unit target = request.target;
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