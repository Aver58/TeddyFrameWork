#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    TimerMgr.cs
 Author:      Zeng Zhiwei
 Time:        2020\12\22 星期二 20:13:50
=====================================================
*/
#endregion

using System.Collections.Generic;
using UnityEngine;

public class TimerMgr:Singleton<TimerMgr>
{
    private List<int> m_lstRemoveTmp = new List<int>();
    private List<ITimerBase> m_lstRunningTimer = new List<ITimerBase>();

    public TimerMgr()
    {
        GameObject go = new GameObject("TimerManager");
        TimerBehaviour bh = go.AddComponent<TimerBehaviour>();
        GameObject.DontDestroyOnLoad(go);
        bh.Init(this);
    }

    // 运行一个定时器
    public void AddTimer(ITimerBase timer)
    {
        if(m_lstRunningTimer.IndexOf(timer) >= 0)
        {
            Debug.Log("已经存在的定时器");
            return;
        }
        m_lstRunningTimer.Add(timer);
    }

    // 停止一个定时器
    public void RemoveTimer(ITimerBase timer)
    {
        int idx = m_lstRunningTimer.IndexOf(timer);
        if(idx >= 0)
        {
            m_lstRunningTimer.RemoveAt(idx);
        }
    }

    public void Update()
    {
        for(int i = m_lstRunningTimer.Count - 1; i >= 0; i--)
        {
            ITimerBase timer = m_lstRunningTimer[i];
            if(timer.IsRunning)
                timer.Update();
            else
                m_lstRemoveTmp.Add(i);
        }

        if(m_lstRemoveTmp.Count > 0)
        {
            for(int i = m_lstRemoveTmp.Count - 1; i >= 0; i--)
            {
                int idx = m_lstRemoveTmp[i];
                ITimerBase timer = m_lstRunningTimer[idx];
                m_lstRunningTimer.RemoveAt(idx);
                timer.Recycle();
            }
            m_lstRemoveTmp.Clear();
        }

    }

}