using System.Collections.Generic;

public class D2Action_Heal : D2Action
{
    private AbilityHealFlag healFlag;
    private AbilityValueSource m_AbilityValueSource;

    public D2Action_Heal(AbilityHealFlag healFlag, AbilityValueSource valueSource, AbilityTarget actionTarget) : base(actionTarget)
    {
        this.healFlag = healFlag;
        m_AbilityValueSource = valueSource;
    }

    protected override void ExecuteByUnit(BattleUnit source, List<BattleUnit> targets)
    {
        for(int i = 0; i < targets.Count; i++)
        {
            var target = targets[i];
            if(target != null)
                AbilityFormula.ApplyHeal(source, target, healFlag, m_AbilityValueSource, abilityData.configFileName);
        }
    }
}
