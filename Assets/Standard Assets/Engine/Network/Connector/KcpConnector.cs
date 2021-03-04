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
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using AOT;
using Framework.Network;

public class KCPConnector : Singleton<KCPConnector>, INetConnector
{
    private int m_connReqTimeout = 3000;  // 连接超时时间，单位毫秒

    public event Action OnConnected;
    public event Action OnDisconnected;
    public event Action<NErrorCode> OnConnectFailed;    // 参数 是否超时的失败类型，KCP超时可以重连，其他不可以

    private IntPtr m_kcp;       // kcp指针
    private uint m_kcpID = 0;
    private Thread m_thread;
    private KcpUdpClient m_udpClient;
    private object m_lockKcp = new object();
    private object m_lockRcv = new object();
    public ConnectState State { get; private set; }
    private ManualResetEvent m_threadResetEvent = new ManualResetEvent(true); // 默认不阻塞
    // udp消息发送队列，主进程需要发送的消息先放入队列，再在线程里面去处理真正的发送
    private Queue<Packet> m_udpSendQueue = new Queue<Packet>();
    // 线程中收好的kcp数据包，会放入这个队列，等待业务层定时来取
    private Queue<byte[]> m_queueRcvData = new Queue<byte[]>();
    private ArrayPool<byte> m_buffers = ArrayPool<byte>.Shared;
    private uint m_runningTime = 0;
    private long m_kcpStartTime = 0;   // kcp开始运行时间
    private FrameTimer m_timerTick;    // tick定时器

    public KCPConnector()
    {
        State = ConnectState.Disconnected;
    }

    public void Init(int timeout)
    {
        m_connReqTimeout = timeout;
        GameLog.Log("超时时间:{0}ms", m_connReqTimeout);
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
        GameLog.Log("Connect,host:{0},port:{1}", ipOrName, port);
        if(State != ConnectState.Disconnected)
            return;

        if(m_timerTick != null)
        {
            GameLog.Log("当前有tick在执行，这不正常");
            m_timerTick.Stop();
        }

        State = ConnectState.Connecting;
        m_udpClient = new KcpUdpClient();
        m_udpClient.SetTimeout(m_connReqTimeout);
        m_udpClient.Connect(ipOrName, port, OnUdpConnected);
    }

    public void Close()
    {
        Clear();
    }

    // 业务层调用的发送
    public void Send(byte[] data)
    {
        GameLog.Log("发送数据，state:{0}, data:{1}", State, data.ToHex());
        if(data == null || State != ConnectState.Connected)
            return;

        lock(m_lockKcp)
            CKcp.KcpSend(m_kcp, data, data.Length);
    }

    // 业务层调用的接收
    public byte[] Receive()
    {
        if(State != ConnectState.Connected)
            return null;

        byte[] bMsg = null;
        lock(m_lockRcv)
        {
            if(m_queueRcvData.Count > 0)
                bMsg = m_queueRcvData.Dequeue();
        }

        return bMsg;
    }

    #endregion

    #region Private

    /// <summary>
    /// 连接服务器回调
    /// </summary>
    private void OnUdpConnected(uint conv, NErrorCode code)
    {
        GameLog.Log("OnUdpConnected,conv:{0}, code:{1}", conv, code);
        if(code == NErrorCode.SUCCESS)
        {
            State = ConnectState.Connected;
            if(m_thread == null)
            {
                // 新建一个线程
                GameLog.Log("[OnUdpConnected]新建一个线程");
                m_thread = new Thread(ThreadProcess);
                m_thread.Start();
            }
            else
            {
                // 恢复线程
                GameLog.Log("[OnUdpConnected]恢复线程");
                m_threadResetEvent.Set();
            }
            m_timerTick = FrameTimer.Create(Tick, 1, -1);
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
        GameLog.Log("kcp线程启动");
        while(true)
        {
            Thread.Sleep(10);
            if(State == ConnectState.Disconnected)
            {
                GameLog.Log("检测到网络断开，线程挂起");
                m_threadResetEvent.WaitOne();
                GameLog.Log("线程恢复");
                return;
            }
            // 接收UDP包 收到的包传入Kcp进行处理
            DealReceiveUdpData();

            // 发送UDP包
            DealSendBuffToUdp();

            if(State == ConnectState.Connected)
            {
                // 接收KCP包
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
                GameLog.Log("kcp收到UDP数据包,sieze:{0},  data:{1}", data.Length, data.ToHex());
                int result = CKcp.KcpInput(m_kcp, data, data.Length);
            }
        }
    }

    // 将发送缓存中的数据发送给Udp
    private void DealSendBuffToUdp()
    {
        Packet packet;
        while(m_udpClient.State == ConnectState.Connected && m_udpSendQueue.Count > 0)
        { 
            packet = m_udpSendQueue.Dequeue();
            GameLog.Log("DealSendBuffToUdp， 发送数据udp，data:" + packet.ToString());
            m_udpClient.Send(packet.Data, packet.Size);
            m_buffers.Return(packet.Data);
        }
    }

    // 处理接收Kcp数据包
    private void DealKcpUpdateAndReceive()
    {
        // kcp 更新 驱动KCPUpdate
        m_runningTime = (uint)(Util.GetCurrMilliSeconds() - m_kcpStartTime);
        lock(m_lockKcp)
            CKcp.KcpUpdate(m_kcp, m_runningTime);

        // kcp receive
        while(true)
        {
            byte[] data;
            int rcvSize, peekSize = 0;
            lock(m_lockKcp)
            {
                peekSize = CKcp.KcpPeeksize(m_kcp);
                if(peekSize <= 0)
                {
                    break;
                }
                data = m_buffers.Rent(peekSize);
                rcvSize = CKcp.KcpRecv(m_kcp, data, peekSize);
            }

            int headSize = data[0] * 256 + data[1] + 2;
            if(rcvSize != peekSize || rcvSize != headSize)
            {
                GameLog.LogWarningFormat("收到的包size验证失败,headSize+2:{0}, realSize:{1}, peekSize:{2}", 
                    headSize, rcvSize, peekSize);
                continue;
            }

            lock(m_lockRcv)
            {
                byte[] realData = new byte[rcvSize - 2];
                Array.Copy(data, 2, realData, 0, rcvSize - 2);
                GameLog.LogFormat("收到kcp处理后的包,放入待接收队列，size:{0}, data:{1}", rcvSize - 2, realData.ToHex());
                m_queueRcvData.Enqueue(realData);
            }
        }
    }

    private void Tick()
    {
        if(m_udpClient.State == ConnectState.Disconnected)
        {
            // 检测到网络断开连接
            if(m_udpClient.DisconnectType == KDisconnectType.FromChange)
            {
                // 来源发生变化的断开连接 重连 挂起kcp线程
                Reconnect();
            }
            else
            {
                // 其他情况提醒外部断开连接 重走登录流程
                if(OnDisconnected != null)
                    OnDisconnected();
                Clear();
            }

        }
    }

    // 重连
    public void Reconnect()
    {
        // 挂起线程
        m_threadResetEvent.Reset();

        m_udpClient.Reconnect(OnUdpReconnected);
    }

    // udp重连回调
    private void OnUdpReconnected(uint conv, NErrorCode code)
    {
        if(code == NErrorCode.SUCCESS)
        {
            State = ConnectState.Connected;
            // 恢复线程
            m_threadResetEvent.Set();
            m_timerTick = FrameTimer.Create(Tick, 1, -1);
        }
        else
        {
            if(code == NErrorCode.ERROR && m_udpClient.DisconnectType == KDisconnectType.FromChange)
            {
                Reconnect();
            }
            else
            {
                ConnectFailed(code);
            }
        }
    }

    // 初始化KCP
    private void InitKCP(uint conv)
    {
        m_kcp = CKcp.KcpCreate(conv, new IntPtr(m_kcpID));
        CKcp.KcpSetoutput(m_kcp, KcpOutput);
        /*参数一 nodelay：
                    0关闭       重发步长为每次加RTO；
                    大于0开启   1重发步为长每次加0.5RTO 2重发步长为每次加0.5实时RTO 
         */
        CKcp.KcpNodelay(m_kcp, 1, 10, 0, 0);
        CKcp.KcpWndsize(m_kcp, 256, 128);
        m_runningTime = 0;
        m_kcpStartTime = Util.GetCurrMilliSeconds();
    }

    // 断开连接
    private void ConnectFailed(NErrorCode code)
    {
        Clear();
        if(OnConnectFailed != null)
            OnConnectFailed(code);
    }

    private void Clear()
    {
        if(State == ConnectState.Disconnected)
            return;

        GameLog.Log("TcpConnector Clear");
        if(m_timerTick != null)
        {
            m_timerTick.Stop();
            m_timerTick = null;
        }

        State = ConnectState.Disconnected;
    }

    // 如果在Unity中启用了ill2cpp，则必须要使用静态方法作为参数传递到C++。
    [MonoPInvokeCallbackAttribute(typeof(KcpOutput))]
    public static int KcpOutput(IntPtr bytes, int len, IntPtr kcp, IntPtr user)
    {
        instance.OnOutput(bytes, len);
        return len;
    }

    // KcpSend之后，会在每个update触发这个OnOutput，这里我们将收到的数据给udp去发送
    public void OnOutput(IntPtr bytes, int size)
    {
        var buff = m_buffers.Rent(size);
        Marshal.Copy(bytes, buff, 0, size);
        Packet packet = new Packet(buff, size);
        //Debug.LogFormat("kcp OnOutput,data:{0}", packet.ToString());
        m_udpSendQueue.Enqueue(packet);
    }

    #endregion
}