#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    D2Event.cs
 Author:      Zeng Zhiwei
 Time:        2020/6/20 14:03:03
=====================================================
*/
#endregion

using System.Collections.Generic;

public class D2Event
{
    private List<D2Action> m_Actions;
    public D2Event(List<D2Action> actions)
    {
        m_Actions = actions;
    }

    public void Execute(BattleEntity source, RequestTarget requestTarget)
    {
        //BattleLog.Log("【D2Event】{0}，source：{1}，target：{2}", GetType().Name, source.GetName(), requestTarget.ToString());

        foreach(D2Action action in m_Actions)
        {
            action.Execute(source, requestTarget);
        }
    }
}