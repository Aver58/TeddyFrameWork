
#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    NewBehaviourScript1.cs
 Author:      Zeng Zhiwei
 Time:        2020/9/13 0:57:13
=====================================================
*/
#endregion

using System.Collections.Generic;

/// <summary>
/// 英雄状态管理
/// </summary>
public class HeroStateController
{
    private HeroState lastState = HeroState.IDLE;
    private HeroState currentState = HeroState.IDLE;
    //private BattleEntity m_BattleEntity;
    private AnimationController m_AnimController;

    public HeroStateController(BattleEntity source, AnimationController animController)
    {
        //m_BattleEntity = source;
        m_AnimController = animController;
    }

    public void Pause()
    {
        if(m_AnimController != null)
        {
            m_AnimController.Pause();
        }
    }

    public void Continue()
    {
        if(m_AnimController != null)
        {
            m_AnimController.Continue();
        }
    }

    public void ChangeHeroState(HeroState newState, string skillName = null, bool isSkipCastPoint = false)
    {
        lastState = currentState;
        currentState = newState;

        HeroStateAction lastActionInfo;
        m_HeroStateChangeMap.TryGetValue(lastState, out lastActionInfo);
        if(lastActionInfo.Exit != null)
            lastActionInfo.Exit(m_AnimController, skillName, isSkipCastPoint);

        HeroStateAction curActionInfo;
        m_HeroStateChangeMap.TryGetValue(currentState, out curActionInfo);
        if(curActionInfo.Enter != null)
            curActionInfo.Enter(m_AnimController, skillName, isSkipCastPoint);
    }

    #region 英雄状态切换map

    private const string idle = "idle";
    private const string run = "run";
    private const string dead = "dead";
    public delegate void StageChangeAction(AnimationController ani, string skillName = null, bool isSkipCastPoint = false);
    private struct HeroStateAction
    {
        public StageChangeAction Enter;
        public StageChangeAction Exit;

        public HeroStateAction(StageChangeAction enter, StageChangeAction exit = null)
        {
            Enter = enter;
            Exit = exit;
        }
    }

    private Dictionary<HeroState, HeroStateAction> m_HeroStateChangeMap = new Dictionary<HeroState, HeroStateAction>
    {
        {HeroState.IDLE,new HeroStateAction(EnterIdleState) },
        {HeroState.MOVE,new HeroStateAction(EnterMoveState) },
        {HeroState.DEAD,new HeroStateAction(EnterDeadState) },
        {HeroState.CASTING,new HeroStateAction(EnterCastingState) },
    };

    private static void SetAnimationControllerTrigger(AnimationController ani,string triggerName)
    {
        if(ani != null)
            ani.SetTrigger(triggerName);
    }

    private static void EnterIdleState(AnimationController ani, string skillName = null, bool isSkipCastPoint = false)
    {
        SetAnimationControllerTrigger(ani, idle);
    }
    private static void EnterMoveState(AnimationController ani, string skillName = null, bool isSkipCastPoint = false)
    {
        SetAnimationControllerTrigger(ani, run);
    }

    private static void EnterDeadState(AnimationController ani, string skillName = null, bool isSkipCastPoint = false)
    {
        // todo 死在空中，需要移到地面来
        SetAnimationControllerTrigger(ani, dead);
    }

    private static void EnterCastingState(AnimationController ani, string skillName = null, bool isSkipCastPoint = false)
    {
        SetAnimationControllerTrigger(ani, skillName);
    }

    #endregion
}