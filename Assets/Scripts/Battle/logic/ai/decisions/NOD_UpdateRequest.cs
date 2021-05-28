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

public class NOD_UpdateRequest : TBTActionLeaf
{
    private ChaseRequest m_chaseRequest;
    private AutoCastAbilityRequest m_autoCastAbilityRequest;

    protected override void onEnter(TBTWorkingData wData)
    {
        base.onEnter(wData);
        BattleDecisionWorkingData decisionData = wData as BattleDecisionWorkingData;
        BattleUnit caster = decisionData.owner;
        // 手动

        // 自动
        Ability ability = caster.SelectCastableAbility();
        if(ability == null)
        {
            // 没有技能就追逐
            var request = decisionData.request;
            if(request == null || (request !=null && request.IsRequestCompleted()))
            {
                var chaseTarget = caster.target;
                if(chaseTarget == null)
                {
                    chaseTarget = TargetSearcher.instance.FindNearestEnemyUnit(caster);
                }

                if(chaseTarget != null)
                {
                    m_chaseRequest = new ChaseRequest(chaseTarget);
                    decisionData.request = m_chaseRequest;
                }
                else
                {
                    //idle
                }
            }
        }
        else
        {
            // 寻找技能目标, 如果有目标被放逐了，可能就选不到目标!
            BattleUnit target = TargetSearcher.instance.FindTargetUnitByAbility(caster,ability);
            if(target!=null)
            {
                BattleLog.Log("【NOD_UpdateRequest】选取到技能ability：{0}，target：{1}", ability.GetConfigName(), target.GetName());
                m_autoCastAbilityRequest = new AutoCastAbilityRequest(ability, target);
                decisionData.request = m_autoCastAbilityRequest;
            }
        }
    }
}