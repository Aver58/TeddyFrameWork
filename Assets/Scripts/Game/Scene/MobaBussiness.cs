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

using UnityEngine;

public class MobaBussiness : Singleton<MobaBussiness>
{
    private HeroActor m_playerActor;

    private BattleUnitManager m_UnitMgr;
    private BattleActorManager m_ActorMgr;
    
    private DebugController m_DrawTool;

    public MobaBussiness()
    {
        GameMsg instance = GameMsg.instance;
        instance.AddMessage<int, Vector3, Vector3>(GameMsgDef.Hero_MoveTo, OnHeroMoveTo);
        instance.AddMessage<int,Vector2>(GameMsgDef.Hero_TurnTo2D, OnHeroTurnTo);
        instance.AddMessage<int,HeroState,string,bool>(GameMsgDef.Hero_ChangeState, OnHeroActorStateChanged);
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
        m_UnitMgr = BattleUnitManager.instance;
        m_ActorMgr = BattleActorManager.instance;
        
        //AddPlayer();
        BattleUnit myEntity = m_UnitMgr.playerUnit;
        m_playerActor = new HeroActor(myEntity);
        m_playerActor.LoadAsset(OnLoadPlayer);

        //AddEnemy();
        var entyties = m_UnitMgr.GetEntities(BattleCamp.ENEMY);
        foreach(BattleUnit unit in entyties)
        {
            HeroActor actor = new HeroActor(unit);
            actor.LoadAsset(OnLoadGuard);
            m_ActorMgr.AddActor(unit.hash,actor);
            // 绘制移动区域
            m_DrawTool.DrawMoveArea(unit.GetStartPoint(), unit.GetViewRange());
        }
    }

    private void OnLoadGuard(GameObject go)
    {
        Transform heroParent = GameObject.Find("GuardNode").transform;
        go.transform.SetParent(heroParent);
    }

    private void OnLoadPlayer(GameObject go)
    {
        m_playerActor.InitPosition(new Vector3(-9, 0, -9));
        Transform heroParent = GameObject.Find("HeroNode").transform;
        go.transform.SetParent(heroParent);
    }

    #region API

    /// <summary>
    /// 增加一个单位
    /// </summary>
    public void AddOneUnit(BattleUnit unit)
    {
        HeroActor actor = new HeroActor(unit);
        actor.LoadAsset(OnLoadDummyUnit);
        m_ActorMgr.AddActor(unit.hash, actor);
    }

    private void OnLoadDummyUnit(GameObject go)
    {
        Transform heroParent = GameObject.Find("GuardNode").transform;
        go.transform.SetParent(heroParent);
        go.transform.position = m_playerActor.transform.position;
    }

    #endregion

    #region EventHandler

    //todo 根据id 筛选
    //todo 为啥不能用 HeorMoveEventArgs
    private void OnHeroMoveTo(int id, Vector3 targetPos, Vector3 toForward)
    {
        foreach(HeroActor render in m_ActorMgr.GetEnemyActors())
        {
            if(render.id == id)
            {
                render.Set3DPosition(targetPos);
            }
        }
    }

    private void OnHeroTurnTo(int id ,Vector2 forward)
    {
        foreach(HeroActor render in m_ActorMgr.GetEnemyActors())
        {
            if(render.id == id)
            {
                render.Set2DForward(forward);
            }
        }
    }

    private void OnHeroActorStateChanged(int heroID, HeroState heroState, string skillName, bool isSkipCastPoint)
    {
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