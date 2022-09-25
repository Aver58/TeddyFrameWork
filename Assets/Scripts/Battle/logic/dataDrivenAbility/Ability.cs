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

// [技能图标](http://mxd.17173.com/content/2013-02-26/20130226150805219.shtml)
using System.Collections.Generic;

public class Ability
{
    public int ID;
    public float CD;
    public int priority;
    public float castTime;
    public BattleUnit caster;
    public AbilityState abilityState;
    public AbilityData abilityData;

    private RequestTarget m_requestTarget;

    public Ability(int id, int priority,BattleUnit battleEntity,AbilityData abilityData)
    {
        ID = id;
        this.priority = priority;
        caster = battleEntity;
        this.abilityData = abilityData;

        castTime = 0f;
        CD = this.abilityData.cooldown;
        abilityState = AbilityState.None;
        m_requestTarget = new RequestTarget();
    }

    public void Update(float deltaTime)
    {
        if(CD > 0)
        {
            CD -= deltaTime;
            if(CD<0)
                CD = 0;
        }

        // 施法的状态切换
        if(abilityState != AbilityState.None)
        {
            castTime += deltaTime;
            float castPoint = abilityData.castPoint;

            // 前摇
            if(abilityState == AbilityState.CastPoint)
            {
                if(castTime > castPoint)
                    OnSpellStart();
            }

            // 持续施法
            if(abilityState == AbilityState.Channeling)
            {
                if(castTime > castPoint + abilityData.channelTime)
                    CastAbilityChannelEnd();
            }

            // 后摇
            if(abilityState == AbilityState.CastBackSwing)
            {
                if(castTime > abilityData.castDuration)
                {
                    castTime = abilityData.castDuration;
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
        CD = abilityData.cooldown;
        if(isSkipCastPoint)
        {
            castTime = abilityData.castPoint;
            // 跳过前摇
            OnSpellStart();
        }
        else
        {
            OnAbilityPhaseStart();

            abilityState = AbilityState.CastPoint;
            if(abilityData.castPoint <= 0)
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
        caster.SetState(HeroState.IDLE);
        // todo 发消息清理特效
    }

    /// <summary>
    /// 施法结束
    /// </summary>
    public void CastAbilityEnd()
    {
        abilityState = AbilityState.None;
        caster.CastAbilityEnd();
    }

    //如果技能处于后摇状态，false 就不管（后摇动作会继续播放,也就是技能会正常结束) true 会被停止(后摇动作会被切换，技能提前结束)
    public void TryBreakAbility(bool forceBreak, AbilityBranch breakAbilityBranch = default)
    {
        if(abilityState == AbilityState.None)
            return;

        if((forceBreak == false) && (abilityData.abilityBranch != breakAbilityBranch))
            return;

        if(abilityState == AbilityState.CastPoint)
            CastAbilityBreak();

        if(abilityState == AbilityState.Channeling)
            CastAbilityChannelEnd();
    }

    #region Event 技能事件

    public void ExecuteEvent(AbilityEvent abilityEvent)
    {
        //BattleLog.Log("【Ability】ExecuteEvent :" + abilityEvent.ToString());

        abilityData.ExecuteEvent(abilityEvent, caster, m_requestTarget);
    }

    // 吟唱阶段
    private void OnAbilityPhaseStart()
    {
        BattleLog.Log("【吟唱阶段】OnAbilityPhaseStart" + abilityData.configFileName);

        abilityState = AbilityState.CastPoint;
        ExecuteEvent(AbilityEvent.OnAbilityPhaseStart);
    }

    // 施法阶段
    private void OnSpellStart()
    {
        BattleLog.Log("【施法阶段】OnSpellStart : {0},channelTime:{1},castPoint:{2}" ,abilityData.configFileName , abilityData.channelTime.ToString(),abilityData.castPoint.ToString());

        if(abilityData.channelTime > 0)
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
        return abilityData.configFileName;
    }

    public float GetCastRange()
    {
        return abilityData.castRange;
    }

    public AbilityUnitAITargetCondition GetAiTargetCondition()
    {
        return abilityData.aiTargetCondition;
    }

    public AbilityBehavior GetAbilityBehavior()
    {
        return abilityData.abilityBehavior;
    }

    public float GetCastleDuring()
    {
        return abilityData.castDuration;
    }

    public ActionTarget GetAbilityTarget()
    {
        return abilityData.ActionTarget;
    }

    public float GetAbilityAOERadius()
    {
        return abilityData.ActionTarget.radius;
    }

    public void GetSectorAoe(out float sectorRadius, out float sectorAngle)
    {
        sectorRadius = abilityData.ActionTarget.sectorRadius;
        sectorAngle = abilityData.ActionTarget.sectorAngle;
    }

    public void GetLineAoe(out float lineLength, out float lineThickness)
    {
        lineLength = abilityData.ActionTarget.lineLength;
        lineThickness = abilityData.ActionTarget.lineThickness;
    }

    public AbilityUnitTargetTeam GetTargetTeam()
    {
        return abilityData.ActionTarget.targetTeam;
    }

    public AbilityUnitTargetType GetTargetType()
    {
        return abilityData.ActionTarget.targetType;
    }

    public string GetCastAnimation()
    {
        return abilityData.castAnimation;
    }

    public float GetTotalCD()
    {
        return abilityData.cooldown;
    }

    public List<ModifierData> GetAllPassiveModifierData()
    {
        return abilityData.GetAllPassiveModifierData();
    }
    #endregion

    #region set

    public void SetUnitTarget(BattleUnit target)
    {
        m_requestTarget.SetUnitTarget(target);
    }

    public void SetPointTarget(float x, float z)
    {
        m_requestTarget.SetPointTarget(x,z);
    }
    
    #endregion
}