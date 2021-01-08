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
    private Dictionary<AbilityCastType,MobaSkillItem> m_MobaSkillItemMap;

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
        m_joystick.onMoveEnd.AddListener(OnMoveEnd);
        //方式一：按键方法注册
        m_joystick.OnPressLeft.AddListener(OnMoving);
        m_joystick.OnPressRight.AddListener(OnMoving);
        m_joystick.OnPressUp.AddListener(OnMoving);
        m_joystick.OnPressDown.AddListener(OnMoving);
    }

    protected override void AddAllMessage()
    {
        base.AddAllMessage();

        GameMsg.instance.AddMessage(GameMsgDef.BattleActor_Created, this, new EventHandler<EventArgs>(OnHeroActorCreated));
        GameMsg.instance.AddMessage(GameMsgDef.PlayerActor_Created, this, new EventHandler<EventArgs>(OnPlayerActorCreated));
        GameMsg.instance.AddMessage(GameMsgDef.Hero_ChangeState, this, new EventHandler<EventArgs>(OnHeroActorStateChanged));
    }

    private void OnHeroActorStateChanged(object sender, EventArgs args)
    {
        HeorChangeStateEventArgs arg = args as HeorChangeStateEventArgs;
        int heroID = arg.id;
        string skillName = arg.skillName;
        HeroState heroState = arg.heroState;
        if(heroState == HeroState.CASTING)
        {
            // 施法,todo，区分玩家和其他玩家
            if(heroID == m_PlayerEntity.id)
            {
                var castType = m_PlayerEntity.GetCastType(skillName);
                MobaSkillItem item;
                m_MobaSkillItemMap.TryGetValue(castType, out item);
                if(item!=null)
                    item.SetCDState();
            }
        }
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

        m_MobaSkillItemMap = new Dictionary<AbilityCastType, MobaSkillItem>();
        List<RectTransform> parents = new List<RectTransform> { nodeAttack , nodeSkill1, nodeSkill2, nodeSkill3 };
        List<AbilityCastType> castTypes = new List<AbilityCastType> { AbilityCastType.ATTACK, AbilityCastType.SKILL1, AbilityCastType.SKILL2, AbilityCastType.SKILL3 };
        for(int i = 0; i < 4; i++)
        {
            var castType = castTypes[i];
            var item = GenerateOne(typeof(MobaSkillItem), prefab, parents[i]) as MobaSkillItem;
            item.Init(castType, m_PlayerEntity.GetAbility(castType), OnItemPointerDown, OnItemDrag, OnItemPointerUp);
            m_MobaSkillItemMap.Add(castType,item);
        }
    }

    private void OnHeroActorCreated(object sender, EventArgs args)
    {
        m_hudActorManager.OnHeroActorCreated(sender, args);
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
        //OnCastAbility(AbilityCastType.ATTACK);
        //按下鼠标左键时
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
            // 如果点击到玩家，就攻击

            if(Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
                // 点击到地板，就移动
                MovePlayerToPoint(hitInfo.point);
            }
        }

        if(Input.GetKeyDown(KeyCode.Q))
            ShowAbilityIndicator(AbilityCastType.ATTACK);
        if(Input.GetKeyUp(KeyCode.Q))
            OnCastAbility(AbilityCastType.ATTACK);

        if(Input.GetKeyDown(KeyCode.W))
            ShowAbilityIndicator(AbilityCastType.SKILL1);
        if(Input.GetKeyDown(KeyCode.W))
            OnCastAbility(AbilityCastType.SKILL1);

        if(Input.GetKeyDown(KeyCode.E))
            ShowAbilityIndicator(AbilityCastType.SKILL2);
        if(Input.GetKeyDown(KeyCode.E))
            OnCastAbility(AbilityCastType.SKILL2);

        if(Input.GetKeyDown(KeyCode.R))
            ShowAbilityIndicator(AbilityCastType.SKILL3);
        if(Input.GetKeyDown(KeyCode.R))
            OnCastAbility(AbilityCastType.SKILL3);

        if(m_cameraManager != null)
            m_cameraManager.Update();
    }

    private void OnItemPointerDown(AbilityCastType abilityCastType)
    {
        Debug.Log("OnItemPointerDown");
        ShowAbilityIndicator(abilityCastType);
    }

    private void OnItemDrag(AbilityCastType abilityCastType, Vector2 vector2)
    {
        Debug.Log("OnItemDrag");
        Debug.RawLog(vector2);
    }

    private void OnItemPointerUp(AbilityCastType abilityCastType)
    {
        Debug.Log("OnItemPointerUp");
        OnCastAbility(abilityCastType);
    }

    private void ShowAbilityIndicator(AbilityCastType castType)
    {
        if(m_PlayerEntity != null)
        {
            Ability ability = m_PlayerEntity.GetAbility(castType);
            if(ability.CD > 0)
            {
                Debug.Log("冷却中");
                return;
            }
            // 技能指示器
            m_PlayerActor.ShowAbilityIndicator(ability);
        }
    }

    private void OnCastAbility(AbilityCastType castType)
    {
        if(m_PlayerEntity != null)
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
}