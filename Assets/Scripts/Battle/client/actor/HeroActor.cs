
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

    public BattleUnit battleUnit { get; }
    public Transform transform { get; private set; }
    public GameObject gameObject { get; private set; }

    private Vector3 m_position = Vector3.zero;
    private DebugController m_DrawTool;
    private AnimationController m_AnimController;
    //private PositionController m_PositionController;
    private HeroStateController m_HeroStateController;
    private Dictionary<AbilityCastType, AbilityActor> m_abilityActorMap;

    protected Action<GameObject> m_LoadedCallback;

    public HeroActor(BattleUnit battleEntity)
    {
        id = battleEntity.GetUniqueID();
        camp = battleEntity.camp;

        isLoadDone = false;
        this.battleUnit = battleEntity;
    }

    public void LoadAsset(Action<GameObject> loadedCallback = null)
    {
        m_LoadedCallback = loadedCallback;
        string path = battleUnit.GetModelPath();
        LoadModule.LoadModel(path, OnLoadComplete);
    }

    public void OnLoadComplete(AssetRequest assetRequest)
    {
        isLoadDone = true;
        GameObject asset = assetRequest.asset as GameObject;

        gameObject = GameObject.Instantiate<GameObject>(asset);
        transform = gameObject.transform;
        //m_PositionController = gameObject.AddComponent<PositionController>();
        // 绘制可视区域
        m_DrawTool = gameObject.AddComponent<DebugController>();
        m_DrawTool.DrawViewArea(battleUnit.GetViewRange());     //可见范围
        m_DrawTool.DrawAttackArea(battleUnit.GetAttackRange()); //攻击范围

        m_AnimController = gameObject.AddComponent<AnimationController>();
        m_HeroStateController = new HeroStateController(battleUnit, m_AnimController);

        InitPosition(Vector3.zero);

        if(m_LoadedCallback != null) 
            m_LoadedCallback(gameObject);

        // todo 找一个区分玩家与友军的标记
        if(camp == BattleCamp.FRIENDLY)
            GameMsg.instance.DispatchEvent(GameMsgDef.PlayerActor_Created, this, camp == BattleCamp.ENEMY);

        gameObject.name = string.Format("[{0}][{1}][UID:{2}][CID:{3}][Lv:{4}][Speed:{5}]",
            battleUnit.GetName(),camp.ToString(),battleUnit.GetUniqueID(),battleUnit.GetID(),battleUnit.GetLevel(),battleUnit.GetMoveSpeed());
        BattleActorManager.instance.AddActor(battleUnit.id, this);

        GameMsg.instance.DispatchEvent(GameMsgDef.BattleActor_Created, this, camp == BattleCamp.ENEMY);
    }

    #region API
    public bool IsDead()
    {
        return battleUnit.IsDead();
    }

    public bool CanMove()
    {
        return battleUnit.IsDead();
    }

    public void ForceUpdateDecisionRequest(BehaviorRequest request)
    {
        battleUnit.ForceUpdateDecisionRequest(request);
    }
    
    public void ChangeState(HeroState newState, string skillName = null, bool isSkipCastPoint = false)
    {
        if(m_HeroStateController != null)
            m_HeroStateController.ChangeHeroState(newState, skillName, isSkipCastPoint);
    }

    public Vector3 Get3DPosition()
    {
        return battleUnit.Get3DPosition();
    }

    public void InitPosition(Vector3 position)
    {
        if(isLoadDone)
        {
            //m_PositionController.InitPosition(position, transform.forward);
            Set3DPosition(position);
            Set3DForward(transform.forward);
        }
    }

    public void Set3DPosition(float x, float y, float z)
    {
        m_position.Set(x, y, z);
        Set3DPosition(m_position);
    }

    public void Set3DPosition(Vector3 position)
    {
        battleUnit.Set3DPosition(position);
        float logicDeltaTime = (position - transform.position).magnitude / battleUnit.GetMoveSpeed();

        //m_PositionController.MoveTo3DPoint(logicDeltaTime, position, OnMoveEnd);
        transform.position = position;
    }

    public void Set2DForward(Vector2 forward)
    {
        m_position.Set(forward.x, transform.position.y, forward.y);
        Set3DForward(m_position);
    }

    public void Set3DForward(Vector3 forward)
    {
        if(forward == Vector3.zero)
            return;
        
        battleUnit.Set3DForward(forward);
        transform.forward = forward;
    }

    public void Set2DForward(float posX, float posZ)
    {
        m_position.Set(posX, transform.position.y, posZ);
        Set3DForward(m_position);
    }

    public void Set3DForward(float posX, float posY, float posZ)
    {
        battleUnit.Set3DForward(posX, posY, posZ);
        m_position.Set(posX, posY, posZ);
        transform.forward = m_position;
    }

    #endregion

    #region Ability Indicator

    private AbilityActor GetAbilityActor(AbilityCastType castType)
    {
        Ability ability = battleUnit.GetAbility(castType);
        if(m_abilityActorMap == null)
            m_abilityActorMap = new Dictionary<AbilityCastType, AbilityActor>(5);

        AbilityActor abilityActor;
        if(m_abilityActorMap.TryGetValue(castType,out abilityActor))
            return abilityActor;

        abilityActor = new AbilityActor(ability,this);
        m_abilityActorMap[castType] = abilityActor;
        return abilityActor;
    }

    // 技能范围
    public void OnFingerDown(AbilityCastType castType)
    {
        var abilityActor = GetAbilityActor(castType);
        abilityActor.OnFingerDown();

        Ability ability = battleUnit.GetAbility(castType);
        // 技能准备动作
        battleUnit.PrepareCastAbility(ability);
    }

    public void OnFingerDrag(AbilityCastType castType, Vector2 mouseDelta)
    {
        var abilityActor = GetAbilityActor(castType);
        abilityActor.OnFingerDrag(mouseDelta);
    }

    public void OnFingerUp(AbilityCastType castType)
    {
        var abilityActor = GetAbilityActor(castType);
        abilityActor.OnFingerUp();
    }

    #endregion
}