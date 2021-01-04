#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    GameMsg.cs
 Author:      Zeng Zhiwei
 Time:        2020/5/23 14:08:46
=====================================================
*/
#endregion

using System.Collections.Generic;

public delegate void EventHandler<TEventArgs>(object sender, TEventArgs e);

// 观察者模式
public class GameMsg : Singleton<GameMsg>
{
    private struct MsgObj
    {
        public MsgObj(object sender, EventHandler<EventArgs> handler)
        {
            this.sender = sender;
            this.handler = handler;
        }
        public object sender;
        public EventHandler<EventArgs> handler;
    }

    private Dictionary<GameMsgDef, List<MsgObj>> m_MsgMap = new Dictionary<GameMsgDef, List<MsgObj>>(); 

    public void AddMessage(GameMsgDef msgName, object sender, EventHandler<EventArgs> callBack)
    {
        List <MsgObj> callbacks = null;
        MsgObj newMsgObj = new MsgObj(sender, callBack);

        if(!m_MsgMap.TryGetValue(msgName, out callbacks))
        {
            callbacks = new List<MsgObj>();
            m_MsgMap[msgName] = callbacks;
        }
        else
        {
            if(callbacks.Contains(newMsgObj))
            {
                Debug.LogWarning("GameMsg.AddMessage duplicate, "+ msgName);
                return;
            }
        }
        callbacks.Add(newMsgObj);
    }

    public void RemoveMessage(GameMsgDef msgName)
    {
        List<MsgObj> callbacks = null;
        if(m_MsgMap.TryGetValue(msgName, out callbacks))
        {
            m_MsgMap.Remove(msgName);
        }
    }

    public void SendMessage(GameMsgDef msgName, EventArgs args)
    {
        List<MsgObj> callbacks = null;
        if(m_MsgMap.TryGetValue(msgName, out callbacks))
        {
            foreach(MsgObj msgObj in callbacks)
            {
                msgObj.handler(msgObj.sender, args);
            }
        }
    }
}