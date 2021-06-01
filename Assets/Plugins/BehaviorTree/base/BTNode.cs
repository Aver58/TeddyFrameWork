#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    BTTreeNode.cs
 Author:      Zeng Zhiwei
 Time:        2021/4/1 17:48:40
=====================================================
*/
#endregion

using System.Collections.Generic;

namespace Aver3
{
    /// <summary>
    /// BT node 是行为树所有节点的基类
    /// </summary>
    public abstract class BTNode
    {
        private BTPrecondition m_precondition;
        public List<BTNode> children { get; }
        public int childCount { get { return children.Count; } }

        public BTNode()
        {
            children = new List<BTNode>();
        }

        protected virtual BTResult OnUpdate(BTData bTData) { return BTResult.Finished; }
        protected virtual bool OnEvaluate(BTData bTData) { return true; }

        private bool IsIndexValid(int index)
        {
            return index >= 0 && index < children.Count;
        }

        #region API

        public bool Evaluate(BTData bTData)
        {
            // 评估这个节点是否可以进入：1.有设置条件；2.条件通过；
            return (m_precondition == null || m_precondition.IsTrue(bTData)) && OnEvaluate(bTData);
        }

        public BTResult Update(BTData bTData)
        {
            return OnUpdate(bTData);
        }

        public void SetPrecondition(BTPrecondition precondition)
        {
            m_precondition = precondition;
        }

        public void AddChild(BTNode node)
        {
            children.Add(node);
        }

        public BTNode GetChild(int index)
        {
            if(!IsIndexValid(index))
                return null;

            return children[index];
        }

        #endregion
    }
}