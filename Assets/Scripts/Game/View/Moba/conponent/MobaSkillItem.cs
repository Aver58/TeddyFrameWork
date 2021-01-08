#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    MobaSkillItem.cs
 Author:      Zeng Zhiwei
 Time:        2021\1\5 星期二 22:47:27
=====================================================
*/
#endregion

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MobaSkillItem : ViewBase
{
    private int m_skillID;
    private Ability m_ability;
    private Action<AbilityCastType> m_downAction;
    private Action<AbilityCastType,Vector2> m_dragAction;
    private Action<AbilityCastType> m_upAction;
    private AbilityCastType m_abilityCastType;
    private Timer timer;
    private TextMeshProUGUI m_txtCD;
    private Image m_imgCDMask;

    public MobaSkillItem(GameObject go,Transform parent):base(go,parent)
    {
        ETCJoystick joystick = (ETCJoystick)UI["Joystick"];
        joystick.onTouchStart.AddListener(OnPointerDown);
        joystick.onTouchUp.AddListener(OnPointerUp);
        joystick.onMove.AddListener(OnDrag);
    }

    public void Init(AbilityCastType castType, Ability ability,Action<AbilityCastType> downAction
        , Action<AbilityCastType, Vector2> dragAction, Action<AbilityCastType> upAction)
    {
        m_downAction = downAction;
        m_dragAction = dragAction;
        m_upAction = upAction;
        m_ability = ability;
        m_skillID = ability.ID;
        m_abilityCastType = castType;

        skillItem skillItem = skillTable.Instance.GetTableItem(m_skillID);
        string iconName = skillItem.icon;
        iconName = "skill/" + iconName;//todo 导出动态图片映射表
        ImageEx ImgIcon = (ImageEx)UI["ImgIcon"];
        ImgIcon.SetSprite(iconName);

        m_txtCD = (TextMeshProUGUI)UI["TxtCD"];
        m_imgCDMask = (Image)UI["ImgCDMask"];

        SetCDState();
    }

    public void SetCDState()
    {
        bool isInCD = m_ability.CD > 0;
        m_imgCDMask.gameObject.SetActive(isInCD);
        timer = Timer.Register(m_ability.CD, OnCDDone, OnCDUpdate, false, true);
    }

    private void OnCDDone()
    {
        Timer.Cancel(timer);
        m_imgCDMask.gameObject.SetActive(false);
    }

    private void OnCDUpdate(float realTime)
    {
        m_imgCDMask.fillAmount = m_ability.CD / m_ability.GetTotalCD();
        m_txtCD.text = m_ability.CD.ToString("f1");
    }

    private void OnPointerDown()
    {
        if(m_downAction != null)
            m_downAction.Invoke(m_abilityCastType);
    }

    private void OnPointerUp()
    {
        if(m_upAction != null)
            m_upAction.Invoke(m_abilityCastType);
    }

    private void OnDrag(Vector2 vector2)
    {
        if(m_dragAction != null)
            m_dragAction.Invoke(m_abilityCastType, vector2);
    }
}