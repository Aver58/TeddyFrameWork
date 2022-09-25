#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    D2Action_Damage.cs
 Author:      Zeng Zhiwei
 Time:        2020/8/31 13:10:06
=====================================================
*/
#endregion

using System.Collections.Generic;

public class D2Action_Damage : D2Action
{
    private AbilityUnitDamageType unitDamageType;
    private AbilityDamageFlag damageFlag;
    private AbilityValueSource m_AbilityValueSource;
    private List<D2Action> m_SuccessActions;

    public D2Action_Damage(AbilityUnitDamageType unitDamageType, AbilityDamageFlag damageFlag, 
        AbilityValueSource valueSource, List<D2Action> successActions, ActionTarget actionTarget) :base(actionTarget)
    {
        this.unitDamageType = unitDamageType;
        this.damageFlag = damageFlag;
        m_SuccessActions = successActions;
        m_AbilityValueSource = valueSource;
    }

    protected override void ExecuteByUnit(BattleUnit source, List<BattleUnit> targets)
    {
        for(int i = 0; i < targets.Count; i++)
        {
            var target = targets[i];
            if(target != null)
                AbilityFormula.ApplyDamage(source, target, unitDamageType, damageFlag, m_AbilityValueSource, abilityData.configFileName);
        }

        //有造成伤害（也就是有攻击到目标）, 执行OnSuccess Actions
        if(m_SuccessActions!=null)
        {
            for(int i = 0; i < m_SuccessActions.Count; i++)
            {
                D2Action action = m_SuccessActions[i];
                action.Execute(source, abilityData, requestTarget);
            }
        }
    }
}