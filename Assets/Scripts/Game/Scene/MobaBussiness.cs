#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    MobaBussiness.cs
 Author:      Zeng Zhiwei
 Time:        2021\1\4 星期一 23:06:28
=====================================================
*/
#endregion

using System.Collections.Generic;
using UnityEngine;

public class MobaBussiness : Singleton<MobaBussiness>
{
    private HeroActor m_playerActor;

    private BattleEntityManager m_EntityMgr;
    private BattleActorManager m_ActorMgr;
    
    private DebugController m_DrawTool;

    public MobaBussiness()
    {
        GameMsg instance = GameMsg.instance;
        instance.AddMessage(GameMsgDef.Hero_MoveTo, (Func<HeorMoveEventArgs>)OnHeroMoveTo);
        instance.AddMessage(GameMsgDef.Hero_TurnTo2D, (Func<HeorTurnEventArgs>)OnHeroTurnTo);
        instance.AddMessage(GameMsgDef.Hero_ChangeState, (Func<HeorChangeStateEventArgs>)OnHeroActorStateChanged);
    }

    ~MobaBussiness()
    {
        GameMsg instance = GameMsg.instance;
        instance.RemoveMessage(GameMsgDef.Hero_MoveTo, this);
        instance.RemoveMessage(GameMsgDef.Hero_TurnTo2D, this);
        instance.RemoveMessage(GameMsgDef.Hero_ChangeState, this);
    }

    public void Init()
    {
        m_DrawTool = GameObject.Find("MoveArea").GetComponent<DebugController>();
        m_EntityMgr = BattleEntityManager.instance;
        m_ActorMgr = BattleActorManager.instance;
        
        //AddPlayer();
        BattleEntity myEntity = m_EntityMgr.GetMyEntity();
        m_playerActor = new HeroActor(myEntity);
        m_playerActor.LoadAsset(OnLoadPlayer);

        //AddEnemy();
        var entyties = m_EntityMgr.GetEntities(BattleCamp.ENEMY);
        foreach(BattleEntity entity in entyties)
        {
            HeroActor actor = new HeroActor(entity);
            actor.LoadAsset(OnLoadGuard);
            m_ActorMgr.AddActor(entity.hash,actor);
            // 绘制移动区域
            m_DrawTool.DrawMoveArea(entity.GetStartPoint(), entity.GetViewRange());
        }
    }

    private void OnLoadGuard(GameObject go)
    {
        Transform heroParent = GameObject.Find("GuardNode").transform;
        go.transform.SetParent(heroParent);
    }

    void OnLoadPlayer(GameObject go)
    {
        m_playerActor.InitPosition(new Vector3(-9, 0, -9));
        Transform heroParent = GameObject.Find("HeroNode").transform;
        go.transform.SetParent(heroParent);
    }

    #region EventHandler

    //todo 根据id 筛选
    //todo 为啥不能用 HeorMoveEventArgs
    private void OnHeroMoveTo(HeorMoveEventArgs args)
    {
        foreach(HeroActor render in m_ActorMgr.GetEnemyActors())
        {
            render.Set3DPosition(args.targetPos);
        }
    }

    private void OnHeroTurnTo(HeorTurnEventArgs args)
    {
        foreach(HeroActor render in m_ActorMgr.GetEnemyActors())
        {
            render.Set2DForward(args.forward);
        }
    }

    private void OnHeroActorStateChanged(HeorChangeStateEventArgs args)
    {
        int heroID = args.id;
        HeroState heroState = args.heroState;
        string skillName = args.skillName;
        bool isSkipCastPoint = args.isSkipCastPoint;

        if(m_playerActor.id == heroID)
        {
            m_playerActor.ChangeState(heroState, skillName, isSkipCastPoint);
        }

        foreach(HeroActor actor in m_ActorMgr.GetEnemyActors())
        {
            if(actor.id == heroID)
            {
                actor.ChangeState(heroState, skillName, isSkipCastPoint);
            }
        }
    }

    #endregion
}