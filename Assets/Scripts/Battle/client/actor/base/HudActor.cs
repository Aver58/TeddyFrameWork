#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    HudActor.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/4 16:26:10
=====================================================
*/
#endregion

using UnityEngine;

public class HudActor
{
    private GameObject m_go;
    private HudController m_hudController;

    public HudActor(GameObject go,HudController hudController)
    {
        m_go = go;
        m_hudController = hudController;
    }

    public void SetValue(float value)
    {
        if(m_hudController)
        {
            m_hudController.SetValue(value);
        }
    }
}