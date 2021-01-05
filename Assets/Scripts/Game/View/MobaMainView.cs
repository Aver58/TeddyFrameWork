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
using UnityEngine.UI;

public class MobaMainView : MainViewBase
{
    public float runSpeed = 0.1f;

    public RectTransform hudParent;
    private ETCJoystick m_joystick;
    private HeroActor m_PlayerActor;
    private BattleEntity m_PlayerEntity;
    private Vector3 moveDistance = Vector3.zero;

    private CameraManager m_cameraManager;
    private HudActorManager m_hudActorManager;

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

        AddListener((Button)UI["BtnMove"], OnBtnMove);
        AddListener((Button)UI["BtnAttack"], delegate { OnCastAbility(AbilityCastType.ATTACK); });
        AddListener((Button)UI["BtnSkill1"], delegate { OnCastAbility(AbilityCastType.SKILL1); });
        AddListener((Button)UI["BtnSkill2"], delegate { OnCastAbility(AbilityCastType.SKILL2); });
        AddListener((Button)UI["BtnSkill3"], delegate { OnCastAbility(AbilityCastType.SKILL3); });

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

    public void OnPlayerActorCreated(object sender, EventArgs args)
    {
        BattleActorCreateEventArgs arg = args as BattleActorCreateEventArgs;
        HeroActor actor = arg.heroActor;

        m_cameraManager.SetWorldCameraPosition(actor.transform.position);
        m_PlayerActor = actor;
        m_PlayerEntity = actor.battleEntity;
    }

    public void OnHeroActorCreated(object sender, EventArgs args)
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
            // 移动相机
            m_cameraManager.SetWorldCameraPosition(position);
        }
    }

    private void OnBtnMove()
    {
        // 鼠标位置转世界坐标位置：发射线，碰撞到的地板的位置
        var uiCamera = CameraManager.instance.uiCamera;

        Vector3 mousePosOnScreen = Input.mousePosition;
        Ray ray = uiCamera.ScreenPointToRay(mousePosOnScreen);
        if(Physics.Raycast(ray, out RaycastHit hitInfo, 10000, LayerMask.NameToLayer("Ground")))
        {
            var hitPos = hitInfo.point;
            MovePlayerToPoint(hitPos);
        }
    }

    void OnCastAbility(AbilityCastType castType)
    {
        if(m_PlayerEntity!=null)
        {
            Ability ability = m_PlayerEntity.GetAbility(castType);
            m_PlayerEntity.CastAbility(ability);
        }
    }

    private void OnMoving()
    {
        if(m_joystick.name != "Joystick")
            return;

        //获取虚拟摇杆偏移量  
        float h = m_joystick.axisX.axisValue;
        float v = m_joystick.axisY.axisValue;

        if(Mathf.Abs(h) > 0.05f || (Mathf.Abs(v) > 0.05f))
        {
            moveDistance.Set(h, 0, v);
            moveDistance *= runSpeed;
            var newPosition = m_PlayerActor.transform.position + moveDistance;
            MovePlayerToPoint(newPosition);
        }
        else
        {
            if(m_PlayerActor!=null)
            {
                m_PlayerActor.ChangeState(HeroState.IDLE);
            }
        }
    }

    private void OnMoveEnd()
    {
        if(m_PlayerActor!=null)
        {
            m_PlayerActor.ChangeState(HeroState.IDLE);
        }
    }

 
}