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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobaMainView : MainViewBase
{
    public float runSpeed = 1f;

    private RectTransform hudParent;
    private ETCJoystick m_joystick;
    private HeroActor m_PlayerActor;
    private BattleEntity m_PlayerEntity;
    private Vector3 moveDistance = Vector3.zero;

    private CameraManager m_cameraManager;
    private HudActorManager m_hudActorManager;
    private List<MobaSkillItem> skillItems;
    private MobaSkillItem attackSkillItem;
    private MobaSkillItem skill1SkillItem;
    private MobaSkillItem skill2SkillItem;
    private MobaSkillItem skill3SkillItem;

    protected override void OnLoaded()
    {
        base.OnLoaded();

        hudParent = (RectTransform)UI["HPHuds"];
        m_cameraManager = CameraManager.instance;
        m_cameraManager.Init();

        m_hudActorManager = HudActorManager.instance;
        m_hudActorManager.Init(hudParent);
    }
    
    protected override void AddAllListener()
    {
        base.AddAllListener();

        AddListener((Button)UI["BtnAttack"], OnBtnAttack);
        
        var JoystickGo = (Image)UI["Joystick"];
        m_joystick = JoystickGo.GetComponent<ETCJoystick>();
        m_joystick.onMoveEnd.AddListener(() => OnMoveEnd());
        m_joystick.onMoveEnd.AddListener(() => OnMoveEnd());
        //方式一：按键方法注册
        m_joystick.OnPressLeft.AddListener(() => OnMoving());
        m_joystick.OnPressRight.AddListener(() => OnMoving());
        m_joystick.OnPressUp.AddListener(() => OnMoving());
        m_joystick.OnPressDown.AddListener(() => OnMoving());
    }

    protected override void AddAllMessage()
    {
        base.AddAllMessage();

        GameMsg.instance.AddMessage(GameMsgDef.BattleActor_Created, this, new EventHandler<EventArgs>(OnHeroActorCreated));
        GameMsg.instance.AddMessage(GameMsgDef.PlayerActor_Created, this, new EventHandler<EventArgs>(OnPlayerActorCreated));
    }

    private void OnPlayerActorCreated(object sender, EventArgs args)
    {
        BattleActorCreateEventArgs arg = args as BattleActorCreateEventArgs;
        HeroActor actor = arg.heroActor;

        m_cameraManager.SetWorldCameraPosition(actor.transform.position);
        m_cameraManager.SetWorldCameraTarget(actor.transform);

        m_PlayerActor = actor;
        m_PlayerEntity = actor.battleEntity;
        var skillIDs = m_PlayerEntity.GetSkillList();
        if(skillIDs == null || skillIDs.Count < 4)
        {
            Debug.LogError("OnPlayerActorCreated player配置的技能列表少于4个！id : "+ m_PlayerEntity.GetID().ToString());
            return;
        }

        var prefab = (GameObject)UI["MobaSkillItem"];
        var nodeAttack = (RectTransform)UI["NodeAttack"];
        var nodeSkill1 = (RectTransform)UI["NodeSkill1"];
        var nodeSkill2 = (RectTransform)UI["NodeSkill2"];
        var nodeSkill3 = (RectTransform)UI["NodeSkill3"];
        attackSkillItem = GenerateOne(typeof(MobaSkillItem), prefab, nodeAttack) as MobaSkillItem;
        skill1SkillItem = GenerateOne(typeof(MobaSkillItem), prefab, nodeSkill1) as MobaSkillItem;
        skill2SkillItem = GenerateOne(typeof(MobaSkillItem), prefab, nodeSkill2) as MobaSkillItem;
        skill3SkillItem = GenerateOne(typeof(MobaSkillItem), prefab, nodeSkill3) as MobaSkillItem;

        attackSkillItem.Init(AbilityCastType.ATTACK, m_PlayerEntity.GetAbility(AbilityCastType.ATTACK), OnClickSkillItem);
        skill1SkillItem.Init(AbilityCastType.SKILL1, m_PlayerEntity.GetAbility(AbilityCastType.SKILL1), OnClickSkillItem);
        skill2SkillItem.Init(AbilityCastType.SKILL2, m_PlayerEntity.GetAbility(AbilityCastType.SKILL2), OnClickSkillItem);
        skill3SkillItem.Init(AbilityCastType.SKILL3, m_PlayerEntity.GetAbility(AbilityCastType.SKILL3), OnClickSkillItem);
    }

    private void OnClickSkillItem(AbilityCastType abilityCastType)
    {
        OnCastAbility(abilityCastType);
    }

    private void OnHeroActorCreated(object sender, EventArgs args)
    {
        m_hudActorManager.OnHeroActorCreated(sender, args);
    }

    private void MovePlayerToPoint(Vector3 position)
    {
        if(m_PlayerActor!=null)
        {
            var forward = position - m_PlayerActor.transform.position;
            m_PlayerActor.Set3DForward(forward);
            m_PlayerActor.Set3DPosition(position);
            m_PlayerActor.ChangeState(HeroState.MOVE);
        }
    }

    private void OnCastAbility(AbilityCastType castType)
    {
        if(m_PlayerEntity!=null)
        {
            Ability ability = m_PlayerEntity.GetAbility(castType);
            if(ability.CD > 0)
            {
                Debug.Log("冷却中");
                return;
            }
            m_PlayerEntity.CastAbility(ability);
        }
    }

    private void OnMoving()
    {
        if(m_joystick.name != "Joystick")
            return;

        //获取虚拟摇杆偏移量  [-1,1]
        float h = m_joystick.axisX.axisValue;
        float v = m_joystick.axisY.axisValue;

        if(Mathf.Abs(h) > 0f || (Mathf.Abs(v) > 0f))
        {
            moveDistance.Set(h, 0, v);
            moveDistance *= runSpeed;
            var newPosition = m_PlayerActor.transform.position + moveDistance;
            MovePlayerToPoint(newPosition);
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
        OnCastAbility(AbilityCastType.ATTACK);
    }
    
    protected override void OnUpdate()
    {
        //按下鼠标右键时
        if(Input.GetMouseButton(1))
        {
            // 鼠标位置转世界坐标位置：发射线，碰撞到的地板的位置
            var worldCamera = CameraManager.instance.worldCamera;
            Vector3 mousePosOnScreen = Input.mousePosition;
            Ray ray = worldCamera.ScreenPointToRay(mousePosOnScreen);
            UnityEngine.Debug.DrawLine(ray.origin, ray.direction, Color.red, 50f);
            if(Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
                MovePlayerToPoint(hitInfo.point);
            }
        }

        if(Input.GetKeyDown(KeyCode.Q))
            OnCastAbility(AbilityCastType.ATTACK);

        if(Input.GetKeyDown(KeyCode.W))
            OnCastAbility(AbilityCastType.SKILL1);

        if(Input.GetKeyDown(KeyCode.E))
            OnCastAbility(AbilityCastType.SKILL2);

        if(Input.GetKeyDown(KeyCode.R))
            OnCastAbility(AbilityCastType.SKILL3);

        if(m_cameraManager != null)
            m_cameraManager.Update();
    }
}