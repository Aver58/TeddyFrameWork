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

using System;
using UnityEngine;
using UnityEngine.UI;
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
    private BaseEventDelegate m_closeCallBack;
    private BlurTextureOnce m_blurTextureOnce;

    public PopupViewBase()
    {
        viewType = ViewType.POPUP;
    }

    protected override void AddAllListener()
    {
        throw new System.NotImplementedException();
    }

    protected override void AddAllMessage()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnClose()
    {
        HideMask();
    }

    protected override void OnLoaded()
    {
        
    }

    protected override void OnOpen(UIEventArgs args = null)
    {
        ShowMask();

        if(!m_bManualMotion)
            PlayOpenMotion();

        if(!m_bManualAnchor)
            SetAnchored();
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

        bool raycast = m_PentrateType == PentrateType.ImPenetrable;
        if(m_UIMask != null)
            m_UIMask.GetComponent<RawImage>().raycastTarget = raycast;

        if(raycast)
        {
            m_closeCallBack = delegate(Transform trans, UnityEngine.EventSystems.BaseEventData eventData) { Close(); };
            ClickListener.AddClick(m_blurTextureOnce, m_closeCallBack);
        }
    }

    private void HideMask()
    {
        if(m_UIMask)
        {
            m_UIMask.transform.localPosition = FarAwayPosition;
            ClickListener.ClearListener(m_blurTextureOnce);
        }
    }

    // 打开动画
    private void PlayOpenMotion()
    {

    }

    // 锚定【左上、中上、右上，左中、中、右中，左下、中下、右下】
    private void SetAnchored()
    {

    }

}