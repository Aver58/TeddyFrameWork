#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    BattleClient.cs
 Author:      Zeng Zhiwei
 Time:        2020\5\26 星期二 0:07:05
=====================================================
*/
#endregion

using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class BattleClient : Singleton<BattleClient>
{
    public HeroActor playerActor;
    private BattleEntityManager m_EntityMgr;
    private List<HeroActor> m_EnemyActors;
    private DebugController m_DrawTool;
    private HudActorManager m_hudActorManager;

    public BattleClient()
    {
        m_EntityMgr = BattleEntityManager.instance;
        m_EnemyActors = new List<HeroActor>();

        m_DrawTool = GameObject.Find("MoveArea").GetComponent<DebugController>();
        
        RegisterMsg();

        m_hudActorManager = new HudActorManager();
    }

    ~BattleClient()
    {
        GameMsg instance = GameMsg.instance;
        instance.RemoveMessage(GameMsgDef.Hero_MoveTo);
        instance.RemoveMessage(GameMsgDef.Hero_TurnTo2D);
        instance.RemoveMessage(GameMsgDef.Hero_ChangeState);
    }

    private void RegisterMsg()
    {
        GameMsg instance = GameMsg.instance;
        instance.AddMessage(GameMsgDef.Hero_MoveTo, this, new EventHandler<EventArgs>(OnHeroMoveTo));
        instance.AddMessage(GameMsgDef.Hero_TurnTo2D, this, new EventHandler<EventArgs>(OnHeroTurnTo));
        instance.AddMessage(GameMsgDef.Hero_ChangeState, this, new EventHandler<EventArgs>(OnHeroActorStateChanged));
    }

    public void Update()
    {

    }

    public void Init()
    {
        //AddPlayer();
        BattleEntity myEntity = m_EntityMgr.GetMyEntity();
        playerActor = new HeroActor(myEntity);
        playerActor.LoadAsset(OnLoadPlayer);
        playerActor.InitPosition(new Vector3(-9,0,-9));
        GameMsg.instance.SendMessage(GameMsgDef.BattleEntity_Created, new BattleActorCreateEventArgs(playerActor, true));

        //AddEnemy();
        var entyties = m_EntityMgr.GetEntities(BattleCamp.ENEMY);
        foreach(BattleEntity entity in entyties)
        {
            HeroActor actor = new HeroActor(entity);
            actor.LoadAsset(OnLoadGuard);
            m_EnemyActors.Add(actor);
            // 绘制移动区域
            m_DrawTool.DrawMoveArea(entity.GetStartPoint(),entity.GetViewRange());
            GameMsg.instance.SendMessage(GameMsgDef.BattleEntity_Created, new BattleActorCreateEventArgs(actor,false));
        }
    }

    private void OnLoadGuard(AssetRequest data)
    {
        GameObject guard = data.asset as GameObject;
        Transform heroParent = GameObject.Find("GuardNode").transform;
        guard.transform.SetParent(heroParent);
    }

    void OnLoadPlayer(AssetRequest data)
    {
        GameObject player = data.asset as GameObject;
        Transform heroParent = GameObject.Find("HeroNode").transform;
        player.transform.SetParent(heroParent);

        InputController inputController = player.AddComponent<InputController>();

        inputController.anim = player.GetComponent<Animator>();
        inputController.characterController = player.GetComponent<CharacterController>();
        inputController.joystick = GameObject.Find("Joystick").GetComponent<ETCJoystick>();
        inputController.btnAttack = GameObject.Find("BtnAttack").GetComponent<Button>();
        inputController.btnSkill1 = GameObject.Find("BtnSkill1").GetComponent<Button>();
        inputController.btnSkill2 = GameObject.Find("BtnSkill2").GetComponent<Button>();
        inputController.btnSkill3 = GameObject.Find("BtnSkill3").GetComponent<Button>();
        inputController.Init(playerActor);
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

        if(playerActor.id == heroID)
        {
            playerActor.ChangeState(heroState, skillName, isSkipCastPoint);
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