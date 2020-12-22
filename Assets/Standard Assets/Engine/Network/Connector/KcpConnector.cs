#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    KcpConnector.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/22 9:53:35
=====================================================
*/
#endregion

public class KCPConnector
{
    public static KCPConnector Instance;

    public KCPConnector()
    {
        Instance = this;
    }

    public void Init(int timeout)
    {
        //m_connReqTimeout = timeout;
        //Log.Info("超时时间:{0}ms", m_connReqTimeout);
    }
}