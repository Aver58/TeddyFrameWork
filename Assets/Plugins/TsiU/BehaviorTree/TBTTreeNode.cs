using System.Collections.Generic;

namespace TsiU
{
    public class TBTTreeNode
    {
        private const int defaultChildCount = -1; //TJQ： 无限数量
        private List<TBTTreeNode> m_children;
        private int m_maxChildCount;

        public TBTTreeNode(int maxChildCount = -1)
        {
            m_children = new List<TBTTreeNode>();
            if (maxChildCount >= 0) {
                m_children.Capacity = maxChildCount;
            }
            m_maxChildCount = maxChildCount;
        }

        public TBTTreeNode(): this(defaultChildCount){}
        ~TBTTreeNode()
        {
            m_children = null;
        }

        public TBTTreeNode AddChild(TBTTreeNode node)
        {
            if (m_maxChildCount >= 0 && m_children.Count >= m_maxChildCount) 
            {
                Debug.LogWarning("**BT** exceeding child count");
                return this;
            }
            m_children.Add(node);
            return this;
        }

        public int GetChildCount()
        {
            return m_children.Count;
        }

        public bool IsIndexValid(int index)
        {
            return index >= 0 && index < m_children.Count;
        }

        public T GetChild<T>(int index) where T : TBTTreeNode 
        {
            if (index < 0 || index >= m_children.Count) 
                return null;

            return (T)m_children[index];
        }
    }
}
