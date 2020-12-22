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

using System;
using System.Threading;
using Framework.Network;

public class KCPConnector : INetConnector
{
    public static KCPConnector Instance;
    private int m_connReqTimeout = 5000;  // 连接超时时间，单位毫秒

    public event Action OnConnected;
    public event Action OnDisconnected;
    public event Action<NErrorCode> OnConnectFailed;    // 参数 是否超时的失败类型，KCP超时可以重连，其他不可以

    public ConnectState State { get; private set; }
    private KcpUdpClient m_udpClient;
    private Thread m_thread;
    private ManualResetEvent m_threadResetEvent = new ManualResetEvent(true); // 默认不阻塞
    private object m_lockKcp = new object();
    private IntPtr m_kcp;       // kcp指针

    public KCPConnector()
    {
        State = ConnectState.Disconnected;
        Instance = this;
    }

    public void Init(int timeout)
    {
        m_connReqTimeout = timeout;
        Debug.Log("超时时间:{0}ms", m_connReqTimeout);
    }

    public void AddListeners(Action connected, Action<NErrorCode> connectedFail, Action disconnected)
    {
        OnConnected = connected;
        OnConnectFailed = connectedFail;
        OnDisconnected = disconnected;
    }

    public void RemoveAllListener()
    {
        OnConnected = null;
        OnConnectFailed = null;
        OnDisconnected = null;
    }

    #region INetConnector

    public void Connect(string ipOrName, int port)
    {
        Debug.Log("Connect,host:{0},port:{1}", ipOrName, port);
        if(State != ConnectState.Disconnected)
            return;

        State = ConnectState.Connecting;
        m_udpClient = new KcpUdpClient();
        m_udpClient.SetTimeout(m_connReqTimeout);
        m_udpClient.Connect(ipOrName, port, OnUdpConnected);
    }

    public void Close()
    {
        throw new NotImplementedException();
    }

    public void Send(byte[] data)
    {
        throw new NotImplementedException();
    }

    public byte[] Receive()
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Private

    /// <summary>
    /// 连接服务器回调
    /// </summary>
    private void OnUdpConnected(uint conv, NErrorCode code)
    {
        Debug.Log("OnUdpConnected,conv:{0}, code:{1}", conv, code);
        if(code == NErrorCode.SUCCESS)
        {
            State = ConnectState.Connected;
            if(m_thread == null)
            {
                // 新建一个线程
                Debug.Log("[OnUdpConnected]新建一个线程");
                m_thread = new Thread(ThreadProcess);
                m_thread.Start();
            }
            else
            {
                // 恢复线程
                Debug.Log("[OnUdpConnected]恢复线程");
                m_threadResetEvent.Set();
            }
            //m_timerTick = CFrameTimer.Create(Tick, 1, -1);
            InitKCP(conv);
            if(OnConnected != null)
                OnConnected();
        }
        else
        {
            ConnectFailed(code);
        }
    }

    // 线程处理函数
    private void ThreadProcess()
    {
        Debug.Log("kcp线程启动");
        while(true)
        {
            Thread.Sleep(10);
            if(State == ConnectState.Disconnected)
            {
                Debug.Log("检测到网络断开，线程挂起");
                m_threadResetEvent.WaitOne();
                Debug.Log("线程恢复");
                return;
            }
            // 处理接收UDP包 收到的包传入Kcp进行处理
            DealReceiveUdpData();

            // 处理UDP发送
            DealSendBuffToUdp();

            // kcp处理
            if(State == ConnectState.Connected)
            {
                // 处理接收KCP包
                DealKcpUpdateAndReceive();
            }
        }
    }

    // 从UDP接收数据进行处理
    private void DealReceiveUdpData()
    {
        while(true)
        {
            byte[] data = m_udpClient.Receive();
            if(data == null)
                break;

            // 数据包处理
            lock(m_lockKcp)
            {
                Debug.Log("kcp收到UDP数据包,sieze:{0},  data:{1}", data.Length, data.ToHex());
                int result = CKcp.KcpInput(m_kcp, data, data.Length);
            }
        }
    }
    #endregion
}