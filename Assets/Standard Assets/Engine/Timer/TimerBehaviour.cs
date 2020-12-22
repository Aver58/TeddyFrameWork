#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    TimerBehaviour.cs
 Author:      Zeng Zhiwei
 Time:        2020\12\22 星期二 20:15:59
=====================================================
*/
#endregion

using UnityEngine;

// todo 有没有其他办法
public class TimerBehaviour : MonoBehaviour
{
    TimerMgr m_timerMgr;
    public void Init(TimerMgr mgr)
    {
        m_timerMgr = mgr;
    }
    void Update()
    {
        m_timerMgr.Update();
    }
}