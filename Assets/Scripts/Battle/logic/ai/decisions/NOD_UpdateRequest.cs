#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    NOD_UpdateRequest.cs
 Author:      Zeng Zhiwei
 Time:        2020\5\18 星期一 23:50:59
=====================================================
*/
#endregion

using TsiU;
using UnityEngine;

public class NOD_UpdateRequest : TBTActionLeaf
{
    private AutoCastAbilityRequest autoCastAbilityRequest;

    protected override void onEnter(TBTWorkingData wData)
    {
        base.onEnter(wData);
        BattleDecisionWorkingData decisionData = wData as BattleDecisionWorkingData;
        BattleEntity caster = decisionData.owner;
        Ability ability = caster.SelectCastableAbility();

        if(ability == null)
        {

        }
        else
        {
            // 寻找技能目标, 如果有目标被放逐了，可能就选不到目标!
            BattleEntity target = TargetSearcher.instance.FindTargetUnitByAbility(caster,ability);
            if(target!=null)
            {
                BattleLog.Log("【NOD_UpdateRequest】选取到技能ability：{0}，target：{1}", ability.GetConfigName(), target.GetName());
                autoCastAbilityRequest = new AutoCastAbilityRequest(ability, target);
                decisionData.request = autoCastAbilityRequest;
            }
        }
    }
}