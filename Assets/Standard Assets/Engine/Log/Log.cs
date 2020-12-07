#region Copyright © 2018 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    Log.cs
 Author:      Zeng Zhiwei
 Time:        2019/12/8 18:04:55
=====================================================
*/
#endregion

public static class Log
{
    public static void Debug(string message)
    {
        UnityEngine.Debug.Log(message);
    }

    public static void DebugFormat(string format,params object[] param)
    {
        UnityEngine.Debug.LogFormat(format, param);
    }

}
