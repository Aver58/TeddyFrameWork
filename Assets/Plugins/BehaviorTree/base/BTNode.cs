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
    /// http://www.aisharing.com/
    /// </summary>
    public abstract class BTNode
    {
        public List<BTNode> children;
        public int childCount { get { return children.Count; } }

        public BTNode()
        {
            children = new List<BTNode>();
        }

        private bool IsIndexValid(int index)
        {
            return index >= 0 && index < children.Count;
        }

        public void AddChild(BTNode node)
        {
            children.Add(node);
        }

        public T GetChild<T>(int index) where T : BTNode
        {
            if(!IsIndexValid(index))
                return default;

            return (T)children[index];
        }
    }
}