#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    Ability.cs
 Author:      Zeng Zhiwei
 Time:        2020/6/17 13:20:32
=====================================================
*/
#endregion

// 技能图标
// http://mxd.17173.com/content/2013-02-26/20130226150805219.shtml
public class Ability
{
    public int ID { get; }
    public float CD { get; set; } = 0f;
    public int priority { get; }
    public float castTime { get; set; }
    public BattleEntity caster { get; }
    public AbilityState abilityState { get; set; }
    public RequestTarget requestTarget { get; set; }
    private AbilityData m_AbilityData;

    public Ability(int id, int priority,BattleEntity battleEntity,AbilityData abilityData)
    {
        ID = id;
        this.priority = priority;
        caster = battleEntity;
        m_AbilityData = abilityData;

        castTime = 0f;
        CD = m_AbilityData.cooldown;
        abilityState = AbilityState.None;
        requestTarget = new RequestTarget();
    }

    public void Update(float deltaTime)
    {
        if(CD > 0)
        {
            CD -= deltaTime;
            if(CD<0)
                CD = 0;
            return;
        }

        // 施法的状态切换
        if(abilityState != AbilityState.None)
        {
            castTime += deltaTime;
            float castPoint = m_AbilityData.castPoint;

            // 前摇
            if(abilityState == AbilityState.CastPoint)
            {
                if(castTime > castPoint)
                    OnSpellStart();
            }

            // 持续施法
            if(abilityState == AbilityState.Channeling)
            {
                if(castTime > castPoint + m_AbilityData.channelTime)
                    CastAbilityChannelEnd();
            }

            // 后摇
            if(abilityState == AbilityState.CastBackSwing)
            {
                if(castTime > m_AbilityData.castDuration)
                {
                    castTime = m_AbilityData.castDuration;
                    CastAbilityEnd();
                }
            }
        }
    }

    /// <summary>
    /// 开始施法
    /// </summary>
    /// <param name="isSkipCastPoint">直接跳过前摇阶段</param>
    public void CastAbilityBegin(bool isSkipCastPoint = false)
    {
        castTime = 0f;
        CD = m_AbilityData.cooldown;
        if(isSkipCastPoint)
        {
            castTime = m_AbilityData.castPoint;
            // 跳过前摇
            OnSpellStart();
        }
        else
        {
            OnAbilityPhaseStart();

            abilityState = AbilityState.CastPoint;
            if(m_AbilityData.castPoint <= 0)
            {
                //没有前摇就直接触发OnSpellStart
                OnSpellStart();
            }
        }
    }

    /// <summary>
    /// 持续施法结束
    /// </summary>
    /// <param name="isInterrupted">是否被打断</param>
    public void CastAbilityChannelEnd(bool isInterrupted = false)
    {
        if(isInterrupted)
        {
            ExecuteEvent(AbilityEvent.OnChannelInterrupted);
            ExecuteEvent(AbilityEvent.OnChannelFinish);
            CastAbilityBreak();
        }
        else
        {
            abilityState = AbilityState.CastBackSwing;
            ExecuteEvent(AbilityEvent.OnChannelFinish);
            ExecuteEvent(AbilityEvent.OnChannelSucceeded);
        }
    }

    /// <summary>
    ///  施法中断
    /// </summary>
    public void CastAbilityBreak()
    {
        abilityState = AbilityState.None;

    }

    /// <summary>
    /// 施法结束
    /// </summary>
    public void CastAbilityEnd()
    {
        abilityState = AbilityState.None;
        caster.CastAbilityEnd();
    }

    #region 技能事件

    public void ExecuteEvent(AbilityEvent abilityEvent)
    {
        //BattleLog.Log("【Ability】ExecuteEvent :" + abilityEvent.ToString());

        m_AbilityData.ExecuteEvent(abilityEvent, caster, requestTarget);
    }

    // 吟唱阶段
    private void OnAbilityPhaseStart()
    {
        BattleLog.Log("【吟唱阶段】OnAbilityPhaseStart" + m_AbilityData.configFileName);

        abilityState = AbilityState.CastPoint;
        ExecuteEvent(AbilityEvent.OnAbilityPhaseStart);
    }

    // 施法阶段
    private void OnSpellStart()
    {
        BattleLog.Log("【施法阶段】OnSpellStart" + m_AbilityData.configFileName);

        if(m_AbilityData.channelTime > 0)
        {
            abilityState = AbilityState.Channeling;
        }
        else
        {
            abilityState = AbilityState.CastBackSwing;
        }

        ExecuteEvent(AbilityEvent.OnSpellStart);
    }

    #endregion

    #region get
    public bool IsCastable()
    {
        return CD <= 0;
        // todo 
        //--被缴械，不能使用物理技能
        //--被沉默，不能使用法术技能
        //-- 被嘲讽魅惑期间，只能释放普通技能
        // 能量不足
    }

    public bool IsUltAbility()
    {
        // todo
        return false;
    }

    public string GetConfigName()
    {
        return m_AbilityData.configFileName;
    }

    public float GetCastleRange()
    {
        return m_AbilityData.castRange;
    }

    public AbilityUnitAITargetCondition GetAiTargetCondition()
    {
        return m_AbilityData.aiTargetCondition;
    }

    public AbilityBehavior GetAbilityBehavior()
    {
        return m_AbilityData.abilityBehavior;
    }

    public float GetCastleDuring()
    {
        return m_AbilityData.castDuration;
    }

    public AbilityUnitTargetTeam GetTargetTeam()
    {
        return m_AbilityData.abilityRange.targetTeam;
    }

    public AbilityAreaDamageType GetDamageType()
    {
        return m_AbilityData.abilityRange.areaDamageType;
    }

    public string GetCastAnimation()
    {
        return m_AbilityData.castAnimation;
    }

    public float GetTotalCD()
    {
        return m_AbilityData.cooldown;
    }
    #endregion
}