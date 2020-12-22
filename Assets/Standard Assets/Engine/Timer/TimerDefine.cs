#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    TimerDefine.cs
 Author:      Zeng Zhiwei
 Time:        2020\12\22 星期二 20:09:13
=====================================================
*/
#endregion

public interface ITimerBase
{
    bool IsRunning { get; }
    void Recycle();
    void Update();
}
