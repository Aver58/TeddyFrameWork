#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    CON_IsAbilityNeedTurnTo.cs
 Author:      Zeng Zhiwei
 Time:        2020/9/7 20:12:16
=====================================================
*/
#endregion

using TsiU;

public class CON_IsAbilityNeedTurnTo : TBTPreconditionLeaf
{
    public override bool IsTrue(TBTWorkingData wData)
    {
        BattleBehaviorWorkingData behaviorData = wData as BattleBehaviorWorkingData;
        float deltaTime = behaviorData.deltaTime;
        BattleEntity source = behaviorData.owner;
        AutoCastAbilityRequest request = behaviorData.request as AutoCastAbilityRequest;
        Entity target = request.target;
        Ability ability = request.ability;

        //目标是自己，不用转身
        if(target == source)
            return false;

        AbilityBehavior abilityBehavior = ability.GetAbilityBehavior();
        //BattleLog.Log("【CON_IsAbilityNeedTurnTo】AbilityBehavior：{0}{1}", abilityBehavior.ToString(), (int)abilityBehavior);

        //无目标技能类型
        if((abilityBehavior & AbilityBehavior.ABILITY_BEHAVIOR_NO_TARGET) != 0)
        {
            //如果是线性或者扇形，虽然是无目标类型，也需要转向目标
            if((abilityBehavior & AbilityBehavior.ABILITY_BEHAVIOR_LINE_AOE) != 0
                || (abilityBehavior & AbilityBehavior.ABILITY_BEHAVIOR_SECTOR_AOE) != 0)
                return true;

            return false;
        }
        return true;
    }
}