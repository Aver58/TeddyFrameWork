#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    D2Modifier.cs
 Author:      Zeng Zhiwei
 Time:        2021\1\29 星期五 23:37:18
=====================================================
*/
#endregion

/// <summary>
/// buff 管理
/// </summary>
public class D2Modifier
{
    public int ID;

    private BattleUnit caster;
    private AbilityData abilityData;
    private BattleUnit target;
    private ModifierData modifierData;
    private RequestTarget requestTarget;

    private bool isEnding;
    private bool isDestroyed;
    private float passedTime;
    private float thinkPassedTime;

    public D2Modifier(BattleUnit caster, ModifierData modifierData, BattleUnit target, AbilityData abilityData)
    {
        this.caster = caster;
        this.target = target;
        this.abilityData = abilityData;
        this.modifierData = modifierData;
        requestTarget = new RequestTarget();
        requestTarget.SetUnitTarget(target);
    }

    // 创建
    public void OnCreate()
    {
        passedTime = 0;
        thinkPassedTime = 0;
        isEnding = false;
        isDestroyed = false;
        
        ExecuteEvent(ModifierEvents.OnCreated);

        ApplyAura();
        // todo 不是傀儡对象
        if(target != null) 
        {
            ApplyProperties();
            ApplyStates();
        }
    }

    public void OnDestroy()
    {
        RemoveAura();
        isDestroyed = true;
        ExecuteEvent(ModifierEvents.OnDestroy);

        if(target != null)
        {
            RemoveProperties();
            RemoveStates();
        }
    }

    public void Update(float deltaTime)
    {
        // 光环
        ApplyAura();

        // 触发持续效果【比如持续掉血】
        var thinkInterval = modifierData.ThinkInterval;
        if(thinkInterval > 0)
        {
            if(thinkPassedTime >= thinkInterval)
            {
                ExecuteEvent(ModifierEvents.OnIntervalThink);
                thinkPassedTime = 0;
            }
            thinkPassedTime += deltaTime;
        }

        // 是否结束
        var duration = modifierData.Duration;
        if((duration != -1) && (passedTime >= duration))
            isEnding = true;

        passedTime += deltaTime;
    }

    public void ExecuteEvent(ModifierEvents eventName)
    {
        var @event = modifierData.GetEvent(eventName);
        if(@event != null)
            @event.Execute(caster, abilityData, requestTarget);
    }

    // 光环
    public void ApplyAura()
    {

    }

    public void RemoveAura()
    {

    }

    // 属性
    public void ApplyProperties()
    {

    }

    public void RemoveProperties()
    {

    }

    // 状态
    public void ApplyStates()
    {

    }

    public void RemoveStates()
    {

    }
}