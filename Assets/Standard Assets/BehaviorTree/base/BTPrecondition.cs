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

    #region 拓展

    public class BTPreconditionNOT : BTPrecondition
    {
        public BTPreconditionNOT(BTPrecondition node)
        {
            AddChild(node);
        }

        public override bool IsTrue(BTData bTData) 
        {
            return !GetChild<BTPrecondition>(0).IsTrue(bTData);
        }
    }

    public abstract class BTPreconditionBinary : BTPrecondition
    {
        public BTPreconditionBinary(BTPrecondition lhs, BTPrecondition rhs)
        {
            AddChild(lhs);
            AddChild(rhs);
        }
    }

    public class BTPreconditionAND : BTPreconditionBinary
    {
        public BTPreconditionAND(BTPrecondition lhs, BTPrecondition rhs) : base(lhs, rhs){}

        public override bool IsTrue(BTData bTData) 
        {
            var child1 = GetChild<BTPrecondition>(0); 
            var child2 = GetChild<BTPrecondition>(1);
            return child1.IsTrue(bTData) && child2.IsTrue(bTData);
        }
    }

    public class BTPreconditionOR : BTPreconditionBinary
    {
        public BTPreconditionOR(BTPrecondition lhs, BTPrecondition rhs) : base(lhs, rhs) { }

        public override bool IsTrue(BTData bTData)
        {
            var child1 = GetChild<BTPrecondition>(0);
            var child2 = GetChild<BTPrecondition>(1);
            return child1.IsTrue(bTData) || child2.IsTrue(bTData);
        }
    }

    #endregion
}