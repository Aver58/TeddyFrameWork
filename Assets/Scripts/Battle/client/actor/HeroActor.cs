
#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    NewBehaviourScript1.cs
 Author:      Zeng Zhiwei
 Time:        2020/9/12 6:06:25
=====================================================
*/
#endregion

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 英雄形象表现层 
/// 含有模型加载，模型位置等数据
/// </summary>
public class HeroActor
{
    public int id;
    public BattleCamp camp;
    public bool isLoadDone;// { get; set; }
    public BattleEntity battleEntity { get; }
    public GameObject gameObject { get; private set; }
    public Transform transform { get; private set; }

    private PositionController m_PositionController;
    private DebugController m_DrawTool;
    private AnimationController m_AnimController;
    private HeroStateController m_HeroStateController;
    private Dictionary<AbilityCastType, AbilityActor> m_abilityActorMap;

    protected Action<GameObject> m_LoadedCallback;

    public HeroActor(BattleEntity battleEntity)
    {
        id = battleEntity.GetUniqueID();
        camp = battleEntity.camp;

        isLoadDone = false;
        this.battleEntity = battleEntity;
    }

    public void LoadAsset(Action<GameObject> loadedCallback = null)
    {
        m_LoadedCallback = loadedCallback;
        string path = battleEntity.GetModelPath();
        LoadModule.LoadModel(path, OnLoadComplete);
    }

    public void OnLoadComplete(AssetRequest assetRequest)
    {
        isLoadDone = true;
        GameObject asset = assetRequest.asset as GameObject;

        gameObject = GameObject.Instantiate<GameObject>(asset);
        transform = gameObject.transform;
        m_PositionController = gameObject.AddComponent<PositionController>();
        // 绘制可视区域
        m_DrawTool = gameObject.AddComponent<DebugController>();
        m_DrawTool.DrawViewArea(battleEntity.GetViewRange());     //可见范围
        m_DrawTool.DrawAttackArea(battleEntity.GetAttackRange()); //攻击范围

        m_AnimController = gameObject.AddComponent<AnimationController>();
        m_HeroStateController = new HeroStateController(battleEntity, m_AnimController);

        InitPosition(Vector3.zero);

        if(m_LoadedCallback != null) 
            m_LoadedCallback(gameObject);

        // todo 找一个区分玩家与友军的标记
        if(camp == BattleCamp.FRIENDLY)
            GameMsg.instance.SendMessage(GameMsgDef.PlayerActor_Created, new BattleActorCreateEventArgs(this, camp == BattleCamp.ENEMY));

        gameObject.name = string.Format("[{0}][{1}][UID:{2}][CID:{3}][Lv:{4}][Speed:{5}]",
            battleEntity.GetName(),camp.ToString(),battleEntity.GetUniqueID(),battleEntity.GetID(),battleEntity.GetLevel(),battleEntity.GetMoveSpeed());
        GameMsg.instance.SendMessage(GameMsgDef.BattleActor_Created, new BattleActorCreateEventArgs(this, camp==BattleCamp.ENEMY));
    }

    public void ChangeState(HeroState newState, string skillName = null, bool isSkipCastPoint = false)
    {
        if(m_HeroStateController != null)
            m_HeroStateController.ChangeHeroState(newState, skillName, isSkipCastPoint);
    }

    public void InitPosition(Vector3 position)
    {
        if(isLoadDone)
        {
            battleEntity.Set3DPosition(position);
            battleEntity.Set3DForward(transform.forward);
            m_PositionController.InitPosition(position, transform.forward);
        }
    }

    public void Set3DPosition(Vector3 position)
    {
        battleEntity.Set3DPosition(position);
        float logicDeltaTime = (position - transform.position).magnitude / battleEntity.GetMoveSpeed();
        m_PositionController.MoveTo3DPoint(logicDeltaTime, position, OnMoveEnd);
    }

    public void Set2DForward(Vector2 position)
    {
        Set3DForward(new Vector3(position.x, transform.position.y, position.y));
    }

    public void Set3DForward(Vector3 position)
    {
        battleEntity.Set3DForward(position);
        transform.forward = position;
    }

    public void OnMoveEnd()
    {
        ChangeState(HeroState.IDLE);
    }

    #region Ability Indicator

    private AbilityActor GetAbilityActor(AbilityCastType castType)
    {
        Ability ability = battleEntity.GetAbility(castType);
        if(m_abilityActorMap == null)
        {
            m_abilityActorMap = new Dictionary<AbilityCastType, AbilityActor>(4);
        }

        AbilityActor abilityActor;
        if(m_abilityActorMap.TryGetValue(castType,out abilityActor))
        {
            return abilityActor;
        }
        abilityActor = new AbilityActor(ability);
        return abilityActor;
    }

    // 技能范围
    public void ShowAbilityRange(AbilityCastType castType)
    {
        var abilityActor = GetAbilityActor(castType);
        abilityActor.ShowAbilityRange();
    }

    public void ShowAbilityIndicator(AbilityCastType castType, Vector3 forward)
    {
        var abilityActor = GetAbilityActor(castType);
        abilityActor.ShowAbilityIndicator(forward);
    }

    #endregion
}