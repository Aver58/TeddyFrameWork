#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    Util.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/22 14:12:29
=====================================================
*/
#endregion

using System;
using System.Text;

public class Util
{
    private static StringBuilder m_stringBuilder = new StringBuilder();
    private static readonly long epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;

    //获取当前时间戳(秒)
    public static long GetCurrTimeStamp()
    {
        return (DateTime.UtcNow.Ticks - epoch) / 10000000;
    }

    // 当前时间戳（毫秒）
    public static long GetCurrMilliSeconds()
    {
        return (DateTime.UtcNow.Ticks - epoch) / 10000;
    }

    public static string GetSpritePath(string name)
    {
        m_stringBuilder.Clear();
        m_stringBuilder.Append(BaseDef.ATLAS_PREFIX);
        m_stringBuilder.Append(name);
        m_stringBuilder.Append(BaseDef.ATLAS_SUFFIX);
        return m_stringBuilder.ToString();
    }
}