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
        private List<BTNode> m_children;
        public int childCount { get { return m_children.Count; } }

        public BTNode()
        {
            m_children = new List<BTNode>();
        }

        private bool IsIndexValid(int index)
        {
            return index >= 0 && index < m_children.Count;
        }

        public void AddChild(BTNode node)
        {
            m_children.Add(node);
        }

        public BTNode GetChild(int index)
        {
            if(!IsIndexValid(index))
                return null;

            return m_children[index];
        }
    }
}