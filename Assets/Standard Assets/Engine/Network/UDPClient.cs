#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    UDPClient.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/26 10:49:05
=====================================================
*/
#endregion

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Network;

/// <summary>
/// 相对tcp的强链接，udp是弱连接，客户端用udp给服务器发消息的时候，
/// 不需要和tcp那样先建立连接（connect）再发送，而是直接向服务器地址发送消息即可。
/// 类似我们发短信，知道对方手机号即可直接发送，而不用关心对方是否开机等信息。
/// 实现方式：一种是用Socket对象，另一种是用UdpClient对象。
/// </summary>
public class UDPClient : Singleton<UDPClient>
{
    public ConnectState State => throw new NotImplementedException();

    //public event Action OnConnected;
    //public event Action OnDisconnected;
    
    private UdpClient m_udpClient;
    private Thread m_connectThread;
    private bool m_isConnected { get; set; }
    //IPEndPoint object will allow us to read datagrams sent from any source.
    private IPEndPoint m_recieveIPEndPoint;

    byte[] m_result = new byte[1024];//存放接收到的消息


    public void Close()
    {
        m_udpClient.Close();
    }

    public void Connect(string ip = "127.0.0.1", int port = 8090)
    {
        // UDP无连接，这边初始化client；
        IPEndPoint ipEnd = new IPEndPoint(IPAddress.Parse(ip), port);
        m_udpClient = new UdpClient();
        m_udpClient.Connect(ipEnd);
        m_isConnected = true;

        //初始化
        m_recieveIPEndPoint = new IPEndPoint(IPAddress.Any, 0);

        //开启一个线程连接
        m_connectThread = new Thread(new ThreadStart(Receive));
        m_connectThread.Start();

        NetMsg.SendUDPMsg(new NetMsgData(OpCode.C2S_TestRequest, "1111"));
    }

    public void Receive()
    {
        while(m_isConnected)
        {
            try
            {
                m_result = m_udpClient.Receive(ref m_recieveIPEndPoint);
                HandleMessage(m_recieveIPEndPoint, m_result);
            }
            catch(Exception)
            {

                throw;
            }
        }
    }

    void HandleMessage(IPEndPoint client, byte[] buffer)
    {
        NetMsg.HandleMsg(buffer);
    }

    public void Send(byte[] data)
    {
        m_udpClient.Send(data,data.Length);
    }
}