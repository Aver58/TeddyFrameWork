#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    BTPrioritySelector.cs
 Author:      Zeng Zhiwei
 Time:        2021/4/2 16:32:32
=====================================================
*/
#endregion

/// <summary>
/// 优先选择节点
/// </summary>
namespace Aver3
{
    public class BTPrioritySelector : BTAction
    {
        private BTAction m_activeChild;

        protected override bool OnEvaluate()
        {
            for(int i = 0; i < childCount; i++)
            {
                var child = GetChild(i) as BTAction;
                if(child.Evaluate())
                {
                    m_activeChild = child;
                    return true;
                }
            }
            return false;
        }

        protected override BTResult OnUpdate()
        {
            if(m_activeChild == null)
                return BTResult.Finished;

            var result = m_activeChild.Update();
            if(result != BTResult.Running)
            {
                m_activeChild = null;
            }
            return result;
        }
    }
}