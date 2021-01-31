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
    public static void ApplyDamage(BattleUnit caster,BattleUnit victim, AbilityDamageType damageType, 
        AbilityDamageFlag damageFlag, AbilityValueSource damageValueSource,string configName)
    {
        int casterLevel = caster.GetLevel();
        BattleProperty casterProperty = caster.GetProperty();
        BattleProperty victimProperty = victim.GetProperty();
        float abilityValue = damageValueSource.GetAbilityValue(casterLevel, casterProperty, victimProperty);
        float finalDamage = CalcDamageByDamageType(abilityValue,damageType, casterProperty, victimProperty);
        // 护盾
        // 吸血

        victim.UpdateHP(-finalDamage);
        BattleLog.LogRpgBattleAttacker(BattleLogic.instance.logicFrame, caster, victim, configName, finalDamage);
    }


    private static float CalcDamageByDamageType(float abilityValue,AbilityDamageType damageType, 
        BattleProperty casterProperty, BattleProperty targetProperty)
    {
        // todo 计算减伤
        switch(damageType)
        {
            case AbilityDamageType.DAMAGE_TYPE_PHYSICAL:

                break;
            case AbilityDamageType.DAMAGE_TYPE_MAGICAL:
                break;
            case AbilityDamageType.DAMAGE_TYPE_PURE:
                break;
            default:
                break;
        }
        return abilityValue;
    }
}