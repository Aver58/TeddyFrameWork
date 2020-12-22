#region Copyright © 2018 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    NetMsg.cs
 Author:      Zeng Zhiwei
 Time:        2019/12/9 14:19:17
=====================================================
*/
#endregion

using System.Collections.Generic;

public delegate void NetHandler(string msgData);

/// <summary>
/// 业务和socket的中间层
/// 业务通过这个类和socket交互
/// </summary>
public static class NetMsg
{
    private static Dictionary<int, NetHandler> m_EventMap = new Dictionary<int, NetHandler>();

    // 向服务器发送请求
    public static void SendMsg(NetMsgData data)
    {
        ClientSocket.Instance.SendMsg(ProtoBufUtil.PackNetMsg(data));
        Log.Info("[Client]client send: ID:{0},Data:{1}", data.ID, data.Data);
    }

    // 派发
    public static void HandleMsg(byte[] buffer)
    {
        NetMsgData data = ProtoBufUtil.UnpackNetMsg(buffer);
        var protoID = data.ID;
        NetHandler callback;
        if (m_EventMap.TryGetValue(protoID, out callback))
        {
            Log.Info("[Server]收到 ：protoID：{0}，data：{1}", protoID, data.Data);
            if (callback!=null)
            {
                callback(data.Data);
            }
        }
        else
        {
            Log.Info("[Server]收到未监听的服务器消息：protoID：{0}，data：{1}", protoID, data.Data);
        }
    }

    // 移除监听
    public static void RemoveListener(int protoID)
    {
        if (m_EventMap.ContainsKey(protoID))
        {
            m_EventMap[protoID] = null;
            m_EventMap.Remove(protoID);
        }
    }

    // 监听 暂时只允许一个地方监听一条服务器消息
    public static void AddListener(int protoID, NetHandler callBack)
    {
        if (!m_EventMap.ContainsKey(protoID))
        {
            m_EventMap.Add(protoID, null);
        }

        m_EventMap[protoID] = callBack;
    }
}