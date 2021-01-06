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

using System.Timers;
using UnityEngine;
using UnityEngine.UI;

public class MobaSkillItem : ViewBase
{
    private int m_skillID;
    private Ability m_ability;
    private System.Action<AbilityCastType> m_action;
    private AbilityCastType m_abilityCastType;
    private Timer timer;
    private Text m_txtCD;
    private Image m_imgCDMask;

    public MobaSkillItem(GameObject go,Transform parent):base(go,parent)
    {
        AddListener((Button)UI["BtnSkill"], OnBtnSkill);
    }

    public void Init(AbilityCastType castType, Ability ability,System.Action<AbilityCastType> action)
    {
        m_action = action;
        m_ability = ability;
        m_skillID = ability.ID;
        m_abilityCastType = castType;

        skillItem skillItem = skillTable.Instance.GetTableItem(m_skillID);
        string iconName = skillItem.icon;
        iconName = "skill/" + iconName;//todo 导出动态图片映射表
        ImageEx ImgIcon = (ImageEx)UI["ImgIcon"];
        ImgIcon.SetSprite(iconName);

        m_txtCD = (Text)UI["TxtCD"];

        SetCDState();
    }

    private void SetCDState()
    {
        m_imgCDMask = (Image)UI["ImgCDMask"];
        bool isInCD = m_ability.CD > 0;
        m_imgCDMask.gameObject.SetActive(isInCD);
        timer = new Timer();
        timer.Elapsed += StartCDCountDown;
        timer.Start();
    }

    private void StartCDCountDown(object sender, ElapsedEventArgs e)
    {
        if(m_ability.CD <= 0)
        {
            timer.Stop();
            m_imgCDMask.gameObject.SetActive(false);
            return;
        }
        m_imgCDMask.fillAmount = m_ability.CD / m_ability.GetTotalCD();
        m_txtCD.text = m_ability.CD.ToString();
    }

    private void OnBtnSkill()
    {
        if(m_action!=null)
            m_action.Invoke(m_abilityCastType);
    }
}