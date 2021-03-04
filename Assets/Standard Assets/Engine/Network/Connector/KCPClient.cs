#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    KCPClient.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/26 16:51:58
=====================================================
*/
#endregion

using System;
using System.Threading;
using KcpProject;

public class KCPClient:Singleton<KCPClient>
{
    private UDPSession m_udpSession;
    private Thread m_connectThread;//接收客户端消息的线程
    byte[] m_result = new byte[1024];//存放接收到的消息
    byte[] buffer = new byte[1500];
    int recvBytes = 0;
    int sendBytes = 0;
    bool stopSend = false;
    int counter = 0;

    public void Connect(string ip, int port)
    {
        m_udpSession = new UDPSession();
        m_udpSession.AckNoDelay = true;
        m_udpSession.WriteDelay = false;

        m_udpSession.Connect(ip, port);
        GameLog.Log("Connet to {0}:{1}",ip,port);
        //开启一个线程连接
        m_connectThread = new Thread(new ThreadStart(Receive));
        m_connectThread.Start();

        //定时器
        //Timer t = new Timer(100);//实例化Timer类，设置间隔时间为3000毫秒
        //t.Elapsed += new ElapsedEventHandler(SendToClient);//到达时间的时候执行事件
        //t.AutoReset = true;//设置是执行一次（false）还是一直执行(true)
        //t.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件
    }

    private void Receive()
    {
        while(true)
        {
            try
            {

                if(!stopSend)
                {
                    GameLog.Log("Write Message...");
                    var send = m_udpSession.Send(buffer, 0, buffer.Length);
                    if(send < 0)
                    {
                        GameLog.Log("Write message failed.");
                        break;
                    }

                    if(send > 0)
                    {
                        counter++;
                        sendBytes += buffer.Length;
                        if(counter >= 500)
                            stopSend = true;
                    }
                }

                var n = m_udpSession.Recv(buffer, 0, buffer.Length);
                if(n == 0)
                {
                    Thread.Sleep(10);
                    continue;
                }
                else if(n < 0)
                {
                    GameLog.Log("Receive Message failed.");
                    break;
                }
                else
                {
                    recvBytes += n;
                    GameLog.Log($"{recvBytes} / {sendBytes}");
                }
            }
            catch(Exception ex)
            {
                GameLog.Log("receive error" + ex.Message);
            }
        }
    }

    public void Send(byte[] data)
    {
        m_udpSession.Send(data, 0, data.Length);
    }
}