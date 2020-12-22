#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    NetDefine.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/22 13:44:47
=====================================================
*/
#endregion

using System;

public enum ConnectState
{
    None,
    Connecting,
    Connected,
    Disconnected,
}

public enum NErrorCode
{
    SUCCESS,
    ERROR,
    TIMEOUT,
}

public enum KConnCmd
{
    ConnReq = 1,    // 连接请求
    ConnRsp = 2,    // 连接应答
    Disconnect = 3, // 断开连接
    Data = 4,       // 数据包
    ReconnReq = 5,  // 重连请求
}

public enum KDisconnectType
{
    Normal = 0,     //默认值
    TimeOut = 1,    // 超时
    Unfound = 2,    // 连接找不到
    FromChange = 3, // ip/port不匹配
    LinkMax = 4,    // 连接数达到上限
    ErrKey = 5,     // 错误的秘钥
}

public interface INetConnector
{
    ConnectState State { get; }

    event Action OnConnected;
    event Action OnDisconnected;

    void Connect(string ipOrName, int port);
    void Close();
    void Send(byte[] data);
    byte[] Receive();
}