
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

    private PositionController m_Mover;
    private DebugController m_DrawTool;
    private AnimationController m_AnimController;
    private HeroStateController m_HeroStateController;
    private Vector3 m_InitPosition = Vector3.zero;
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
        m_Mover = gameObject.AddComponent<PositionController>();
        // 绘制可视区域
        m_DrawTool = gameObject.AddComponent<DebugController>();
        m_DrawTool.DrawViewArea(battleEntity.GetViewRange());     //可见范围
        m_DrawTool.DrawAttackArea(battleEntity.GetAttackRange()); //攻击范围

        m_AnimController = gameObject.AddComponent<AnimationController>();
        m_HeroStateController = new HeroStateController(battleEntity, m_AnimController);

        if(m_LoadedCallback != null) 
            m_LoadedCallback(gameObject);

        InitPosition(m_InitPosition);

        gameObject.name = string.Format("[{0}][{1}][UID:{2}][CID:{3}][Lv:{4}][Speed:{5}]",
            battleEntity.GetName(),camp.ToString(),battleEntity.GetUniqueID(),battleEntity.GetID(),battleEntity.GetLevel(),battleEntity.GetMoveSpeed());
        GameMsg.instance.SendMessage(GameMsgDef.BattleEntity_Created, new BattleActorCreateEventArgs(this, camp==BattleCamp.ENEMY));
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
            gameObject.transform.position = position;
            battleEntity.Set3DForward(gameObject.transform.forward);
        }
        m_InitPosition = position;
    }

    public void Set3DPosition(Vector3 position)
    {
        battleEntity.Set3DPosition(position);
        gameObject.transform.position = position;
    }

    public void Set2DForward(Vector2 forward)
    {
        gameObject.transform.forward = new Vector3(forward.x, 0, forward.y);
    }
}