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
        instance.RegisterListener<int, float, float, float>(GameMsgDef.Hero_MoveTo, OnHeroMoveTo);
        instance.RegisterListener<int, float, float, float>(GameMsgDef.Hero_TurnTo3D, OnHeroTurnTo);
        instance.RegisterListener<int,HeroState,string,bool>(GameMsgDef.Hero_ChangeState, OnHeroActorStateChanged);
    }

    ~MobaBussiness()
    {
        GameMsg instance = GameMsg.instance;
        instance.UnRegisterListener(GameMsgDef.Hero_MoveTo, this);
        instance.UnRegisterListener(GameMsgDef.Hero_TurnTo3D, this);
        instance.UnRegisterListener(GameMsgDef.Hero_ChangeState, this);
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
            // 绘制移动区域
            m_DrawTool.DrawMoveArea(unit.GetStartPoint(), unit.GetViewRange());
        }
    }

    private void OnLoadGuard(GameObject go)
    {
        Transform parent = GameObject.Find("GuardNode").transform;
        go.transform.SetParent(parent);
    }

    private void OnLoadPlayer(GameObject go)
    {
        m_playerActor.InitPosition(Vector3.zero);
        Transform parent = GameObject.Find("HeroNode").transform;
        go.transform.SetParent(parent);
    }

    #region API

    /// <summary>
    /// 增加一个单位
    /// </summary>
    public void AddOneUnit(BattleUnit unit)
    {
        HeroActor actor = new HeroActor(unit);
        actor.LoadAsset(OnLoadDummyUnit);
    }

    private void OnLoadDummyUnit(GameObject go)
    {
        Transform heroParent = GameObject.Find("GuardNode").transform;
        go.transform.SetParent(heroParent);
        go.transform.position = m_playerActor.transform.position;
    }

    #endregion

    #region EventHandler

    private void OnHeroMoveTo(int id, float x, float y, float z)
    {
        var actor = m_ActorMgr.GetActor(id);
        if(actor!=null)
            actor.Set3DPosition(x,y,z);
    }

    private void OnHeroTurnTo(int id, float x, float y, float z)
    {
        var actor = m_ActorMgr.GetActor(id);
        if(actor != null)
            actor.Set3DForward(x,y,z);
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