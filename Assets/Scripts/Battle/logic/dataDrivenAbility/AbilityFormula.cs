#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    AbilityFormula.cs
 Author:      Zeng Zhiwei
 Time:        2020/8/31 20:59:32
=====================================================
*/
#endregion

public static class AbilityFormula
{
    // 造成伤害
    public static void ApplyDamage(BattleUnit caster, BattleUnit victim, AbilityUnitDamageType unitDamageType,
        AbilityDamageFlag damageFlag, AbilityValueSource damageValueSource, string configName)
    {
        int casterLevel = caster.GetLevel();
        BattleProperty casterProperty = caster.GetProperty();
        BattleProperty victimProperty = victim.GetProperty();
        float abilityValue = damageValueSource.GetAbilityValue(casterLevel, casterProperty, victimProperty);
        float finalDamage = CalcDamageByDamageType(abilityValue, unitDamageType, casterProperty, victimProperty);
        // 护盾
        // 吸血

        victim.UpdateHP(-finalDamage);
        BattleLog.LogRpgBattleAttacker(BattleLogic.Instance.logicFrame, caster, victim, configName, finalDamage);
    }

    // 计算不同伤害类型造成的伤害
    private static float CalcDamageByDamageType(float abilityValue, AbilityUnitDamageType unitDamageType,
        BattleProperty casterProperty, BattleProperty targetProperty)
    {
        // todo 计算减伤
        switch(unitDamageType)
        {
            case AbilityUnitDamageType.DAMAGE_TYPE_PHYSICAL:

                break;
            case AbilityUnitDamageType.DAMAGE_TYPE_MAGICAL:
                break;
            case AbilityUnitDamageType.DAMAGE_TYPE_PURE:
                break;
            default:
                break;
        }
        return abilityValue;
    }

    // 治疗
    public static void ApplyHeal(BattleUnit caster, BattleUnit target, AbilityHealFlag healFlag, AbilityValueSource damageValueSource, string configName)
    {
        int casterLevel = caster.GetLevel();
        BattleProperty casterProperty = caster.GetProperty();
        BattleProperty victimProperty = target.GetProperty();
        float abilityValue = damageValueSource.GetAbilityValue(casterLevel, casterProperty, victimProperty);
        var finalValue = abilityValue;

        target.UpdateHP(finalValue);
        //BattleLog.LogRpgBattleHealer(BattleLogic.instance.logicFrame, caster, target, configName, finalValue);
    }
}