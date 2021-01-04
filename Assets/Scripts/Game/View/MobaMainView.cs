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
    public float runSpeed = 2f;

    public Transform hudParent;
    private ETCJoystick m_joystick;
    private CharacterController m_characterController;
    private CameraManager m_cameraManager;
    private HeroActor m_PlayerActor;
    private BattleEntity m_PlayerEntity;
    private Vector3 moveDistance = Vector3.zero;

    protected override void OnLoaded()
    {
        base.OnLoaded();

        hudParent = (Transform)UI["HPHuds"];
        AddListener((Button)UI["BtnMove"], OnBtnMove);
        AddListener((Button)UI["BtnAttack"], delegate { OnCastAbility(AbilityCastType.ATTACK); });
        AddListener((Button)UI["BtnSkill1"], delegate { OnCastAbility(AbilityCastType.SKILL1); });
        AddListener((Button)UI["BtnSkill2"], delegate { OnCastAbility(AbilityCastType.SKILL2); });
        AddListener((Button)UI["BtnSkill3"], delegate { OnCastAbility(AbilityCastType.SKILL3); });

        m_joystick = (ETCJoystick)UI["Joystick"];
        m_joystick.onMoveEnd.AddListener(() => OnMoveEnd());
        m_joystick.onMoveEnd.AddListener(() => OnMoveEnd());
        //方式一：按键方法注册
        m_joystick.OnPressLeft.AddListener(() => OnMoving());
        m_joystick.OnPressRight.AddListener(() => OnMoving());
        m_joystick.OnPressUp.AddListener(() => OnMoving());
        m_joystick.OnPressDown.AddListener(() => OnMoving());

        m_cameraManager = CameraManager.instance;
    }

    protected override void AddAllMessage()
    {
        base.AddAllMessage();
        GameMsg.instance.AddMessage(GameMsgDef.BattleEntity_Created, this, new EventHandler<EventArgs>(OnHeroActorCreated));
    }

    public void OnHeroActorCreated(object sender, EventArgs args)
    {
        BattleActorCreateEventArgs arg = args as BattleActorCreateEventArgs;
        HeroActor actor = arg.heroActor;

        m_characterController = actor.gameObject.GetComponent<CharacterController>();
        m_cameraManager.SetPosition(actor.gameObject.transform.position);
        m_PlayerActor = actor;
        m_PlayerEntity = actor.battleEntity;
    }

    private void MovePlayer2Point(Vector3 position)
    {
        if(m_PlayerActor!=null)
        {
            // 旋转
            transform.LookAt(position);
            m_characterController.Move(position);
            m_PlayerActor.Set3DPosition(position);
            m_PlayerActor.ChangeState(HeroState.MOVE);
            // 移动相机
            m_cameraManager.SetPosition(position);
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
            MovePlayer2Point(hitPos);
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
            var newPosition = transform.position + moveDistance;
            MovePlayer2Point(newPosition);
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