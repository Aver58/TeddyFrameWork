#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    TestKCP.cs
 Author:      Zeng Zhiwei
 Time:        2020\12\25 星期五 22:26:44
=====================================================
*/
#endregion

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using KcpProject;

//https://www.jianshu.com/p/9544c2e56757
public class TestKCP
{
    public static void Test()
    {
        var connection = new UDPSession();
        connection.AckNoDelay = true;
        connection.WriteDelay = false;

        connection.Connect("127.0.0.1", 8090);

        //var stopSend = false;
        //var buffer = new byte[1500];
        //var counter = 0;
        //var sendBytes = 0;
        //var recvBytes = 0;

        //while(true)
        //{
        //    connection.Update();

        //    if(!stopSend)
        //    {
        //        Debug.Log("Write Message...");
        //        var send = connection.Send(buffer, 0, buffer.Length);
        //        if(send < 0)
        //        {
        //            Debug.Log("Write message failed.");
        //            break;
        //        }

        //        if(send > 0)
        //        {
        //            counter++;
        //            sendBytes += buffer.Length;
        //            if(counter >= 500)
        //                stopSend = true;
        //        }
        //    }

        //    var n = connection.Recv(buffer, 0, buffer.Length);
        //    if(n == 0)
        //    {
        //        Thread.Sleep(10);
        //        continue;
        //    }
        //    else if(n < 0)
        //    {
        //        Debug.Log("Receive Message failed.");
        //        break;
        //    }
        //    else
        //    {
        //        recvBytes += n;
        //        Debug.Log($"{recvBytes} / {sendBytes}");
        //    }

        //    var resp = Encoding.UTF8.GetString(buffer, 0, n);
        //    Debug.Log("Received Message: " + resp);
        //}
    }
}