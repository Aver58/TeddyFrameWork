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

using System;
using System.Collections.Generic;


//函数
public delegate void Func();
public delegate void Func<T>(T t1);
public delegate void Func<T1, T2>(T1 t1, T2 t2);
public delegate void Func<T1, T2, T3>(T1 t1, T2 t2, T3 t3);
//函数带返回值
public delegate T FuncRet<T>();
public delegate T FuncRet<T, T1>(T1 t1);
public delegate T FuncRet<T, T1, T2>(T1 t1, T2 t2);
public delegate T FuncRet<T, T1, T2, T3>(T1 t1, T2 t2, T3 t3);

// 观察者模式
public class GameMsg : Singleton<GameMsg>
{
    private Dictionary<GameMsgDef, Delegate> m_MsgMap = new Dictionary<GameMsgDef, Delegate>();

    public void AddMessage(GameMsgDef msgName, Delegate listener)
    {
        if(listener == null)
            return;

        Delegate func = null;
        if(m_MsgMap.TryGetValue(msgName, out func))
            Delegate.Combine(func, listener);
        else
            func = listener;

        m_MsgMap[msgName] = func;
    }

    // todo test
    public void RemoveMessage(GameMsgDef msgName, object sender)
    {
        if(sender == null)
            return;
        
        Delegate funcs = null;
        if(m_MsgMap.TryGetValue(msgName, out funcs))
        {
            foreach(Delegate func in funcs.GetInvocationList())
            {
                if(func.Target == sender)
                {
                    Delegate.Remove(funcs, func);
                }
            }
        }

        if(funcs == null)
            m_MsgMap.Remove(msgName);
    }

    #region SendMessage

    // 无参
    public void SendMessage(GameMsgDef msgName)
    {
        Delegate funcs = null;
        if(m_MsgMap.TryGetValue(msgName, out funcs))
        {
            foreach(Delegate func in funcs.GetInvocationList())
            {
                Func tmp = (Func)func;
                tmp();
            }
        }
    }

    // 1个参
    public void SendMessage<T>(GameMsgDef msgName,T arg)
    {
        Delegate funcs = null;
        if(m_MsgMap.TryGetValue(msgName, out funcs))
        {
            foreach(Delegate func in funcs.GetInvocationList())
            {
                Func<T> tmp = (Func<T>)func;
                tmp(arg);
            }
        }
    }

    // 2个参
    public void SendMessage<T1, T2>(GameMsgDef msgName, T1 arg1, T2 arg2)
    {
        Delegate funcs = null;
        if(m_MsgMap.TryGetValue(msgName, out funcs))
        {
            foreach(Delegate func in funcs.GetInvocationList())
            {
                Func<T1, T2> tmp = (Func<T1, T2>)func;
                tmp(arg1,arg2);
            }
        }
    }

    // 3个参
    public void SendMessage<T1, T2, T3>(GameMsgDef msgName, T1 arg1, T2 arg2, T3 arg3)
    {
        Delegate funcs = null;
        if(m_MsgMap.TryGetValue(msgName, out funcs))
        {
            foreach(Delegate func in funcs.GetInvocationList())
            {
                Func<T1, T2, T3> tmp = (Func<T1, T2, T3>)func;
                tmp(arg1, arg2, arg3);
            }
        }
    }

    #endregion
}