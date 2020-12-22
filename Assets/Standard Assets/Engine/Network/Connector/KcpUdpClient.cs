#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    KcpUdpClient.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/22 13:51:12
=====================================================
*/
#endregion

using System;
using System.Buffers;
using System.Net;
using System.Net.Sockets;
/// <summary>
/// 管理和服务器的连接状态 线程安全
/// </summary>
public class KcpUdpClient
{
    public ConnectState State { get; private set; }
    public KDisconnectType DisconnectType { get; private set; }
    private int m_connReqTimeout = 5;  // 连接超时时间，单位秒

    private string m_host;
    private int m_port;
    public string Host { get { return m_host; } }
    public int Port { get { return m_port; } }
    private Action<uint, NErrorCode> m_onConnected;
    private UdpClient m_udp;
    /// <summary>
    /// 连接ID
    /// </summary>
    private static int s_flowID = 0;
    private long m_startConnTime = 0; // 开始连接服务器时间
    private ArrayPool<byte> m_buffers = ArrayPool<byte>.Shared;
    private object m_lockUdp = new object();

    public KcpUdpClient()
    {
        State = ConnectState.Disconnected;
        DisconnectType = KDisconnectType.Normal;
    }

    #region API

    public void SetTimeout(int timeout)
    {
        m_connReqTimeout = timeout;
    }

    public void Connect(string ipOrName, int port, Action<uint, NErrorCode> callback)
    {
        m_onConnected = callback;
        m_host = ipOrName;
        m_port = port;

        // UDP端口绑定
        IPAddress[] addr = Dns.GetHostAddresses(ipOrName);
        if(addr == null || addr.Length == 0)
        {
            m_onConnected(0, NErrorCode.ERROR);
            return;
        }

        State = ConnectState.Connecting;
        m_udp = new UdpClient(addr[0].AddressFamily);
        m_udp.Connect(addr[0], port);

        // 发送连接请求
        SendConnectRequest();
        return;
    }

    #endregion

    #region Private

    /// <summary>
    /// 发送连接请求
    /// </summary>
    private void SendConnectRequest()
    {
        s_flowID++;
        short size = 6;
        Packet packet = new Packet() { Data = m_buffers.Rent(size), Size = size };
        packet.WriteShort((short)(size - 2));
        packet.WriteShort((short)KConnCmd.ConnReq);
        packet.WriteShort((short)s_flowID);

        m_startConnTime = Util.GetCurrMilliSeconds();

        // 直接UdpSocket发送
        Debug.Log("SendConnectRequest, flowid:{0}, time:{1}, packet:{2}", s_flowID, Util.GetCurrMilliSeconds(), packet.ToString());

        Send(packet.Data, packet.Size);
        m_buffers.Return(packet.Data);
    }

    // 发送字节流
    public void Send(byte[] data, int size)
    {
        if(data == null || m_udp == null)
            return;

        lock(m_lockUdp)
            m_udp.Send(data, size);
    }

    // 接收
    public byte[] Receive()
    {
        if(m_udp == null)
            return null;

        byte[] data = null;
        IPEndPoint endPoint = null;
        lock(m_lockUdp)
        {
            if(m_udp.Available > 0)
            {
                data = m_udp.Receive(ref endPoint);
                if(State == ConnectState.Connected && data.Length < 24)
                {
                    // 如果是连接包 直接内部处理了
                    DealCtrlPkgOfConnected(data);
                    data = null;
                }
            }
        }
        return data;
    }

    // 连接过程中的控制包 目前只有服务器主动断开连接
    private void DealCtrlPkgOfConnected(byte[] data)
    {
        KCPPacket packet = new KCPPacket(data, data.Length);
        KCPPacket.KOperateStruct optInfo = packet.ParseOperatePkg();
        if(optInfo == null)
            return;

        if(optInfo.cmd == KConnCmd.Disconnect && State == ConnectState.Connected)
        {
            // 服务器主动断开连接
            DisconnectType = (KDisconnectType)optInfo.content;
            State = ConnectState.Disconnected;
        }
    }
    #endregion
}