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

/// <summary>
/// 弹窗类型
/// </summary>
public class PopupBase : ViewBase
{
    protected bool m_bManualAnchor = false;
    protected bool m_bManualMotion = false;
    public PopupBase()
    {
        viewType = ViewType.POPUP;
    }

    protected override void AddListeners()
    {
        throw new System.NotImplementedException();
    }

    protected override void AddMessages()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnClose()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnLoaded()
    {
        throw new System.NotImplementedException();
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

    }

    private void HideMask()
    {

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