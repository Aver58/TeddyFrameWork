#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    BTPrecondition.cs
 Author:      Zeng Zhiwei
 Time:        2021/4/2 11:04:37
=====================================================
*/
#endregion

namespace Aver3
{
    /// <summary>
    /// 条件节点
    /// </summary>
    public abstract class BTPrecondition : BTNode
    {
        public abstract bool IsTrue(BTData bTData);
    }
}