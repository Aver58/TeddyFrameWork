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

using UnityEngine;
using UnityEngine.UI;

public class MobaSkillItem : ViewBase
{
    private int m_skillID;
    private Ability m_ability;
    private System.Action<AbilityCastType> m_action;
    private AbilityCastType m_abilityCastType;

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

        Image ImgCDMask = (Image)UI["ImgCDMask"];
        ImgCDMask.gameObject.SetActive(false);
    }

    private void OnBtnSkill()
    {
        if(m_action!=null)
            m_action.Invoke(m_abilityCastType);
    }
}