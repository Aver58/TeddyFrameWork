#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    MainViewBase.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/14 14:55:37
=====================================================
*/
#endregion

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 模糊类型
/// </summary>
public enum LucencyType
{
    Lucency,//透明
    Translucence,//半透明
}

/// <summary>
/// 穿透类型
/// </summary>
public enum PentrateType
{
    Pentrate,//能穿透
    ImPenetrable,//不能穿透
}

/// <summary>
/// 锚定点【左上、中上、右上，左中、中、右中，左下、中下、右下】
/// </summary>
public enum AnchoringPoint
{
    LeftTop,
    MiddleTop,
    RightTop,
    LeftMiddle,
    Middle,
    RightMiddle,
    LeftBottom,
    MiddleBottom,
    RightBottom,
}


/// <summary>
/// 弹窗类型
/// </summary>
public class PopupViewBase : ViewBase
{
    protected bool m_bManualAnchor = false;
    protected bool m_bManualMotion = false;
    protected LucencyType m_LucencyType = LucencyType.Translucence;
    protected PentrateType m_PentrateType = PentrateType.ImPenetrable;

    private GameObject m_UIMask;
    private BlurTextureOnce m_blurTextureOnce;
    private bool m_raycast { get { return m_PentrateType == PentrateType.ImPenetrable; } }
    public PopupViewBase()
    {
        viewType = ViewType.POPUP;
    }

    protected override void OnClose()
    {
        HideMask();
    }

    protected override void OnOpen(object[] args)
    {
        ShowMask();

        if(!m_bManualMotion)
            PlayOpenMotion();

        if(!m_bManualAnchor)
            SetAnchoringPoint();
    }

    private void ShowMask()
    {
        if(m_LucencyType == LucencyType.Translucence)
        {
            if(m_blurTextureOnce == null)
            {
                m_UIMask = UIModule.GetUIMask();

                m_blurTextureOnce = m_UIMask.GetComponent<BlurTextureOnce>();
                m_blurTextureOnce.Init();
            }
            m_UIMask.transform.localPosition = Vector3.zero;

            // 移走自己，以防被相机拍到
            MoveFarAway(true);
            m_blurTextureOnce.GenerateRender();
            MoveFarAway(false);
        }

        if(m_UIMask != null)
            m_UIMask.GetComponent<RawImage>().raycastTarget = m_raycast;

        if(m_raycast)
        {
            ClickListener.AddClick(m_blurTextureOnce, OnClickMask);
        }
    }

    private void OnClickMask(Transform trans, BaseEventData eventData)
    {
        Close();
    }

    private void HideMask()
    {
        if(m_UIMask)
        {
            m_UIMask.transform.localPosition = FarAwayPosition;
        }

        if(m_raycast)
        {
            ClickListener.ClearListener(m_blurTextureOnce);
        }
    }

    // 打开动画
    private void PlayOpenMotion()
    {

    }

    // 锚定点
    private void SetAnchoringPoint(AnchoringPoint anchoringPoint = AnchoringPoint.Middle)
    {

    }
}