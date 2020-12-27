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
    private float maxRadius;
    private float startPointX, startPointZ, endPointX, endPointZ;
    private double distanceSquare;
    private DummyEntity dummyEntity = new DummyEntity(-999,0,0);
    private IdleRequest idleRequest = new IdleRequest();
    private ChaseRequest chaseRequest = new ChaseRequest(null);
    private AutoCastAbilityRequest autoCastAbilityRequest;

    protected override void onEnter(TBTWorkingData wData)
    {
        base.onEnter(wData);
        BattleDecisionWorkingData decisionData = wData as BattleDecisionWorkingData;
        BattleEntity caster = decisionData.owner;
        Ability ability = caster.SelectCastableAbility();

        if(ability == null)
        {
            //// 没有目标的话，尝试寻找目标
            //if(caster.target == null)
            //{
            //    // todo 这个应该给enemy做个回家的node和回家的request
            //    caster.GetStartPoint(out startPointX, out startPointZ);
            //    BattleEntity enemy = TargetSearcher.instance.FindNearestEnemyUnit(caster);
            //    if(enemy != null)
            //    {
            //        enemy.Get2DPosition(out endPointX, out endPointZ);
            //        distanceSquare = BattleMath.Distance2DMagnitude(startPointX, startPointZ, endPointX, endPointZ);
            //        maxRadius = enemy.GetViewRange();
            //        if(distanceSquare > maxRadius * maxRadius)
            //        {
            //            Debug.Log("敌人的距离超过出生点太远，就不追了，回家: " + startPointX + startPointZ);
            //            dummyEntity.Set2DPosition(startPointX, startPointZ);
            //            chaseRequest.SetTarget(dummyEntity);
            //            decisionData.request = chaseRequest;
            //        }
            //        else
            //        {
            //            Debug.Log("追击敌人！");
            //            chaseRequest.SetTarget(enemy);
            //            decisionData.request = chaseRequest;
            //        }
            //    }
            //    else
            //    {
            //        caster.Get2DPosition(out endPointX, out endPointZ);
            //        distanceSquare = BattleMath.Distance2DMagnitude(startPointX, startPointZ, endPointX, endPointZ);
            //        if(distanceSquare > 0)
            //        {
            //            Debug.Log("没有对象的时候，回家: " + startPointX + startPointZ);
            //            dummyEntity.Set2DPosition(startPointX, startPointZ);
            //            chaseRequest.SetTarget(dummyEntity);
            //            decisionData.request = chaseRequest;
            //        }
            //        else
            //        {
            //            Debug.Log("在家");
            //            decisionData.request = idleRequest;
            //        }
            //    }
            //}
            //else
            //{
            //    Debug.Log("有对象");
            //    decisionData.request = idleRequest;
            //}
        }
        else
        {
            // 寻找技能目标, 如果有目标被放逐了，可能就选不到目标!
            BattleEntity target = TargetSearcher.instance.FindTargetUnitByAbility(caster,ability);
            if(target!=null)
            {
                BattleLog.Log("【自动战斗】ability：{0}，target：{1}", ability.GetConfigName(), target.GetName());
                autoCastAbilityRequest = new AutoCastAbilityRequest(ability, target);
                decisionData.request = autoCastAbilityRequest;
            }
        }
    }
}