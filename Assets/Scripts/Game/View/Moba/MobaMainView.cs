#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    MobaMainView.cs
 Author:      Zeng Zhiwei
 Time:        2021/1/4 20:15:39
=====================================================
*/
#endregion

using UnityEngine;
using System.Collections.Generic;

public partial class MobaMainView : MainViewBase
{
    public float runSpeed = 1f;

    private ETCJoystick m_joystick;
    private HeroActor m_PlayerActor;
    private BattleUnit m_PlayerUnit;
    private Vector3 moveDistance = Vector3.zero;

    private CameraManager m_cameraManager;
    private HudActorManager m_hudActorManager;
    private Dictionary<AbilityCastType,MobaSkillItem> m_MobaSkillItemMap;

    private DummyUnit m_cacheDummyUnit;
    private ChaseRequest m_cacheChaseRequest;

    protected override void OnLoaded()
    {
        base.OnLoaded();

        m_cameraManager = CameraManager.instance;
        m_cameraManager.Init();

        m_hudActorManager = HudActorManager.instance;
        m_hudActorManager.Init(HPHuds);

        InitCheatPanel();

        AddOneDummyUnit();

        m_cacheDummyUnit = new DummyUnit(-1, -1);
        m_cacheChaseRequest = new ChaseRequest(m_cacheDummyUnit);
    }

    #region Cheat

    private void InitCheatPanel()
    {
        AddOneCheatButton("添加人偶", AddOneDummyUnit);
        AddOneCheatButton("添加AI", AddOneEnemyUnit);
    }

    private void AddOneCheatButton(string text, System.Action callBack)
    {
        var item = GenerateOne(typeof(CheatButtonItem), CheatButtonItem, GridLayout) as CheatButtonItem;
        item.Init(text, callBack);
    }

    // 添加一个人偶对象
    private void AddOneDummyUnit()
    {
        var unit = BattleLogic.instance.AddOneDummyUnit();
        MobaBussiness.instance.AddOneUnit(unit);
    }

    private void AddOneEnemyUnit()
    {
        var unit = BattleLogic.instance.AddOneEnemyUnit();
        MobaBussiness.instance.AddOneUnit(unit);
    }

    #endregion

    protected override void AddAllListener()
    {
        base.AddAllListener();

        AddListener(BtnAttack, OnBtnAttack);

        m_joystick = JoystickLeft;
        m_joystick.onMove.AddListener(OnMoving);
        m_joystick.onMoveEnd.AddListener(OnMoveEnd);
    }

    protected override void AddAllMessage()
    {
        base.AddAllMessage();

        GameMsg.instance.AddMessage<HeroActor, bool>(GameMsgDef.BattleActor_Created,OnHeroActorCreated);
        GameMsg.instance.AddMessage<HeroActor, bool>(GameMsgDef.PlayerActor_Created, OnPlayerActorCreated);
        GameMsg.instance.AddMessage<int, string>(GameMsgDef.Hero_Cast_Ability, OnHeroCastAbility);
    }

    private MobaSkillItem GetSkillItem(AbilityCastType castType)
    {
        MobaSkillItem item;
        m_MobaSkillItemMap.TryGetValue(castType, out item);
        return item;
    }

    private void OnHeroCastAbility(int id, string skillName)
    {
        if(m_PlayerActor.id == id)
        {
            var castType = m_PlayerUnit.GetCastType(skillName);
            MobaSkillItem item = GetSkillItem(castType);
            if(item != null)
                item.SetCDState();
        }
    }

    private void OnPlayerActorCreated(HeroActor actor, bool isFriend)
    {
        m_cameraManager.SetWorldCameraPosition(actor.transform.position);
        m_cameraManager.SetWorldCameraTarget(actor.transform);

        m_PlayerActor = actor;
        m_PlayerUnit = actor.battleUnit;
        var skillIDs = m_PlayerUnit.GetSkillList();
        if(skillIDs == null || skillIDs.Count < 4)
        {
            GameLog.LogError("OnPlayerActorCreated player配置的技能列表少于4个！id : "+ m_PlayerUnit.GetID().ToString());
            return;
        }

        m_MobaSkillItemMap = new Dictionary<AbilityCastType, MobaSkillItem>();
        List<RectTransform> parents = new List<RectTransform> { NodeAttack, NodeSkill1, NodeSkill2, NodeSkill3 };
        List<AbilityCastType> castTypes = new List<AbilityCastType> { AbilityCastType.ATTACK, AbilityCastType.SKILL1, AbilityCastType.SKILL2, AbilityCastType.SKILL3 };
        for(int i = 0; i < 4; i++)
        {
            var castType = castTypes[i];
            var item = GenerateOne(typeof(MobaSkillItem), MobaSkillItem, parents[i]) as MobaSkillItem;
            item.Init(castType, m_PlayerUnit.GetAbility(castType), OnFingerDown, OnFingerDrag, OnFingerUp);
            m_MobaSkillItemMap.Add(castType,item);
        }
    }

    private void OnHeroActorCreated(HeroActor actor, bool isFriend)
    {
        m_hudActorManager.OnHeroActorCreated(actor, isFriend);
    }

    private void MovePlayerToPoint(Vector3 position)
    {
        if(m_PlayerActor != null)
        {
            var forward = position - m_PlayerActor.transform.position;
            if(forward != Vector3.zero)
                m_PlayerActor.Set3DForward(forward);

            m_PlayerActor.Set3DPosition(position);
            m_PlayerActor.ChangeState(HeroState.MOVE);
        }
    }

    private void OnMoving(Vector2 axisValue)
    {
        if(m_joystick.name != "JoystickLeft")
            return;

        if(m_PlayerActor.IsDead())
            return;

        //获取虚拟摇杆偏移量  [-1,1]
        float h = axisValue.x;
        float v = axisValue.y;
        if(Mathf.Abs(h) > 0f || (Mathf.Abs(v) > 0f))
        {
            moveDistance.Set(h, 0, v);
            moveDistance *= runSpeed;
            var newPosition = m_PlayerActor.transform.position + moveDistance;
            // MovePlayerToPoint(newPosition);
            // todo 改成请求,发个移动请求到指定位置的请求到行为树里 todo 缓存请求和unit

            m_cacheDummyUnit.Set3DPosition(newPosition.x,0,newPosition.z);
            //m_cacheChaseRequest.target = m_cacheDummyUnit;
            m_PlayerActor.ForceUpdateDecisionRequest(m_cacheChaseRequest);
        }
        else
        {
            if(m_PlayerActor!=null)
                m_PlayerActor.ChangeState(HeroState.IDLE);
        }
    }

    private void OnMoveEnd()
    {
        if(m_PlayerActor!=null)
            m_PlayerActor.ChangeState(HeroState.IDLE);
    }

    private void OnBtnAttack()
    {
        //OnCastAbility(AbilityCastType.ATTACK);
        //按下鼠标左键时
    }

    protected override void OnUpdate()
    {
        if(m_PlayerActor.IsDead())
            return;

        //按下鼠标右键时
        if(Input.GetMouseButton(1))
        {
            // 鼠标位置转世界坐标位置：发射线，碰撞到的地板的位置
            var worldCamera = CameraManager.instance.worldCamera;
            Vector3 mousePosOnScreen = Input.mousePosition;
            Ray ray = worldCamera.ScreenPointToRay(mousePosOnScreen);
            Debug.DrawLine(ray.origin, ray.direction, Color.red, 50f);
            // 如果点击到玩家，就攻击

            if(Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
                // 点击到地板，就移动
                MovePlayerToPoint(hitInfo.point);
            }
        }

        if(m_cameraManager != null)
            m_cameraManager.Update();
    }

    private void OnFingerDown(AbilityCastType abilityCastType)
    {
        if(m_PlayerActor.IsDead())
            return;

        if(m_PlayerActor != null)
            m_PlayerActor.OnFingerDown(abilityCastType);
    }

    private void OnFingerDrag(AbilityCastType abilityCastType, Vector2 mouseDelta)
    {
        if(m_PlayerActor.IsDead())
            return;

        if(m_PlayerActor != null)
            m_PlayerActor.OnFingerDrag(abilityCastType, mouseDelta);
    }

    private void OnFingerUp(AbilityCastType abilityCastType)
    {
        if(m_PlayerActor.IsDead())
            return;

        if(m_PlayerActor != null)
            m_PlayerActor.OnFingerUp(abilityCastType);
    }
}