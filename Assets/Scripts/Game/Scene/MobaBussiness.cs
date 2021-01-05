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

public class MobaBussiness:Singleton<MobaBussiness>
{
    private HeroActor m_playerActor;

    private BattleEntityManager m_EntityMgr;
    private List<HeroActor> m_EnemyActors;
    private DebugController m_DrawTool;

    public MobaBussiness()
    {
        GameMsg instance = GameMsg.instance;
        instance.AddMessage(GameMsgDef.Hero_MoveTo, this, new EventHandler<EventArgs>(OnHeroMoveTo));
        instance.AddMessage(GameMsgDef.Hero_TurnTo2D, this, new EventHandler<EventArgs>(OnHeroTurnTo));
        instance.AddMessage(GameMsgDef.Hero_ChangeState, this, new EventHandler<EventArgs>(OnHeroActorStateChanged));
    }

    ~MobaBussiness()
    {
        GameMsg instance = GameMsg.instance;
        instance.RemoveMessage(GameMsgDef.Hero_MoveTo);
        instance.RemoveMessage(GameMsgDef.Hero_TurnTo2D);
        instance.RemoveMessage(GameMsgDef.Hero_ChangeState);
    }

    public void Init()
    {
        m_DrawTool = GameObject.Find("MoveArea").GetComponent<DebugController>();
        m_EnemyActors = new List<HeroActor>();
        m_EntityMgr = BattleEntityManager.instance;

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
            m_EnemyActors.Add(actor);
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
    private void OnHeroMoveTo(object sender, EventArgs args)
    {
        HeorMoveEventArgs arg = args as HeorMoveEventArgs;
        foreach(HeroActor render in m_EnemyActors)
        {
            render.Set3DPosition(arg.targetPos);
        }
    }

    private void OnHeroTurnTo(object sender, EventArgs args)
    {
        HeorTurnEventArgs arg = args as HeorTurnEventArgs;
        foreach(HeroActor render in m_EnemyActors)
        {
            render.Set2DForward(arg.forward);
        }
    }

    private void OnHeroActorStateChanged(object sender, EventArgs args)
    {
        HeorChangeStateEventArgs arg = args as HeorChangeStateEventArgs;
        int heroID = arg.id;
        HeroState heroState = arg.heroState;
        string skillName = arg.skillName;
        bool isSkipCastPoint = arg.isSkipCastPoint;

        if(m_playerActor.id == heroID)
        {
            m_playerActor.ChangeState(heroState, skillName, isSkipCastPoint);
        }

        foreach(HeroActor actor in m_EnemyActors)
        {
            if(actor.id == heroID)
            {
                actor.ChangeState(heroState, skillName, isSkipCastPoint);
            }
        }
    }

    #endregion
}