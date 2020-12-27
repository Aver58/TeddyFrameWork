#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    AbilityValueSource.cs
 Author:      Zeng Zhiwei
 Time:        2020/8/31 13:55:29
=====================================================
*/
#endregion

using System.Collections.Generic;

public class AbilityValueSource
{
    public float baseValue { get; set; }
    public float baseValueGrow { get; set; }

    private struct AdditionSourceValue
    {
        public AdditionSourceValue(AbilityValueSourceType abilityValueSourceType, int sourceValueCoefficient, int sourceValueCoefficientGrow, int limitBasicValue, int limitBasicValueGrow)
        {
            this.abilityValueSourceType = abilityValueSourceType;
            this.sourceValueCoefficient = sourceValueCoefficient;
            this.sourceValueCoefficientGrow = sourceValueCoefficientGrow;
            this.limitBasicValue = limitBasicValue;
            this.limitBasicValueGrow = limitBasicValueGrow;
        }

        /// <summary>
        /// 技能数值来源
        /// </summary>
        public AbilityValueSourceType abilityValueSourceType { get; set; }
        /// <summary>
        /// 技能基础系数
        /// </summary>
        public int sourceValueCoefficient { get; set; }
        /// <summary>
        /// 技能成长系数
        /// </summary>
        public int sourceValueCoefficientGrow { get; set; }
        public int limitBasicValue { get; set; }
        public int limitBasicValueGrow { get; set; }
    }

    private List<AdditionSourceValue> m_AdditionSourceValues { get; set; }

    private static Dictionary<AbilityValueSourceType, System.Func<BattleProperty, BattleProperty, int>> GetAdditionSourceValueFuncMap = new Dictionary<AbilityValueSourceType, System.Func<BattleProperty, BattleProperty, int>> 
    {
        {AbilityValueSourceType.SOURCE_TYPE_PHYSICAL_ATTACK,delegate(BattleProperty casterProperty, BattleProperty targetProperty){ return casterProperty.GetPhysic(); } },
        {AbilityValueSourceType.SOURCE_TYPE_MAGICAL_ATTACK, delegate(BattleProperty casterProperty, BattleProperty targetProperty){ return casterProperty.GetMagic(); } },
        {AbilityValueSourceType.SOURCE_TYPE_CASTER_MAX_HP, delegate(BattleProperty casterProperty, BattleProperty targetProperty){ return casterProperty.GetMaxHP(); } },
        {AbilityValueSourceType.SOURCE_TYPE_CASTER_CURRENT_HP, delegate(BattleProperty casterProperty, BattleProperty targetProperty){ return casterProperty.curHP; } },
        {AbilityValueSourceType.SOURCE_TYPE_CASTER_LOST_HP, delegate(BattleProperty casterProperty, BattleProperty targetProperty){ return casterProperty.GetMaxHP() - casterProperty.curHP; } },
        {AbilityValueSourceType.SOURCE_TYPE_TARGET_MAX_HP, delegate(BattleProperty casterProperty, BattleProperty targetProperty){ return targetProperty.GetMaxHP(); } },
        {AbilityValueSourceType.SOURCE_TYPE_TARGET_CURRENT_HP, delegate(BattleProperty casterProperty, BattleProperty targetProperty){ return targetProperty.curHP; } },
        {AbilityValueSourceType.SOURCE_TYPE_TARGET_LOST_HP, delegate(BattleProperty casterProperty, BattleProperty targetProperty){ return targetProperty.GetMaxHP() - targetProperty.curHP; } },
    };
    public AbilityValueSource()
    {
        m_AdditionSourceValues = new List<AdditionSourceValue>(0);
    }

    public void AddAdditionValueParams(AbilityValueSourceType abilityValueSourceType, int sourceValueCoefficient, int sourceValueCoefficientGrow, int limitBasicValue, int limitBasicValueGrow)
    {
        AdditionSourceValue additionSourceValue = new AdditionSourceValue(abilityValueSourceType,
                  sourceValueCoefficient, sourceValueCoefficientGrow, limitBasicValue, limitBasicValueGrow);
        m_AdditionSourceValues.Add(additionSourceValue);
    }

    public float GetAbilityValue(int casterLevel,BattleProperty casterProperty, BattleProperty targetProperty)
    {
        float abilityValue = baseValue + baseValueGrow * casterLevel;
        // additionSourceValue 附加值
        if(m_AdditionSourceValues.Count > 0)
        {
            for(int i = 0; i < m_AdditionSourceValues.Count; i++)
            {
                AdditionSourceValue data = m_AdditionSourceValues[i];
                AbilityValueSourceType abilityValueSourceType = data.abilityValueSourceType;
                int sourceValueCoefficient = data.sourceValueCoefficient;
                int sourceValueCoefficientGrow = data.sourceValueCoefficientGrow;
                int limitBasicValue = data.limitBasicValue;
                int limitBasicVlimitBasicValueGrowalue = data.limitBasicValueGrow;

                float coefficient = sourceValueCoefficient * 0.0001f + casterLevel * sourceValueCoefficientGrow * 0.0001f;
                int additionSourceValue = GetAdditionSourceValueFuncMap[abilityValueSourceType](casterProperty, targetProperty);
                float addition = additionSourceValue * coefficient;

                abilityValue += addition;
            }
        }
        return abilityValue;
    }
}