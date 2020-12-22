#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    FrameTimer.cs
 Author:      Zeng Zhiwei
 Time:        2020\12\22 星期二 20:08:56
=====================================================
*/
#endregion

using System;
using UnityEngine;
/// <summary>
/// 帧定时器
/// </summary>
public class FrameTimer : ITimerBase
{
    private int m_loop;
    private int m_count = 0;
    private Action m_callback;
    private int m_durationCount; // 间隔次数
    private bool m_running = false;
    private static ObjectPool<FrameTimer> s_poolTimer = new ObjectPool<FrameTimer>();

    public bool IsRunning { get { return m_running; } }

    public static FrameTimer Create(Action callback, int duration = 1, int loop = -1)
    {
        FrameTimer timer = s_poolTimer.Get();
        timer.Reset(callback, duration, loop);
        timer.Start();
        return timer;
    }

    // 重置定时器
    private void Reset(Action callback, int duration, int loop)
    {
        m_callback = callback;
        m_durationCount = duration;
        m_loop = loop;
        m_running = false;
        m_count = Time.frameCount + duration;
    }

    public void Recycle()
    {
        Reset(null, 0, 0);
        s_poolTimer.Release(this);
    }

    // 启动定时器
    private void Start()
    {
        m_running = true;
        TimerMgr.instance.AddTimer(this);
    }

    // 停止定时器
    public void Stop()
    {
        m_running = false;
    }

    public void Update()
    {
        if(!m_running)
            return;

        int frameCount = Time.frameCount;
        if(frameCount >= m_count)
        {
            if(m_callback != null)
                m_callback();

            m_loop--;
            if(m_loop == 0)
                Stop();
            else
                m_count = frameCount + m_durationCount;
        }
    }
}