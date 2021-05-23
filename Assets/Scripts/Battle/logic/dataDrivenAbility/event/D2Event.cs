#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    D2Event.cs
 Author:      Zeng Zhiwei
 Time:        2020/6/20 14:03:03
=====================================================
*/
#endregion

using System.Collections.Generic;

/// <summary>
/// 技能事件
/// Each of these keys is a block that can trigger actions.
/// </summary>
/*
OnAbilityEndChannel
OnAbilityPhaseStart -- Triggers when the ability is cast (before the unit turns toward the target)
OnAbilityStart
OnAttack
OnAttackAllied
OnAttackFailed
OnChannelFinish
OnChannelInterrupted
OnChannelSucceeded
OnCreated
OnEquip
OnHealReceived
OnHealthGained
OnHeroKilled
OnManaGained
OnOrder
OnOwnerDied
OnOwnerSpawned
OnProjectileDodge
OnProjectileFinish
OnProjectileHitUnit -- Adding the KV pair "DeleteOnHit" "0" in this block will cause the projectile to not disappear when it hits a unit.
OnRespawn
OnSpellStart
OnSpentMana
OnStateChanged
OnTeleported
OnTeleporting
OnToggleOff
OnToggleOn
OnUnitMoved
OnUpgrade
     */
public class D2Event
{
    private List<D2Action> m_Actions;
    public D2Event(List<D2Action> actions)
    {
        m_Actions = actions;
    }

    public void Execute(BattleUnit source, AbilityData abilityData, RequestTarget requestTarget)
    {
        BattleLog.Log("【D2Event】{0}，source：{1}，target：{2}", GetType().Name, source.GetName(), requestTarget.ToString());
        foreach(D2Action action in m_Actions)
        {
            action.Execute(source, abilityData, requestTarget);
        }
    }
}