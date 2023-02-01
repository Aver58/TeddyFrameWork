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
/// 每次都是自左向右依次选择，当发现找到一个可执行的子节点后就停止搜索后续子节点。
/// </summary>
namespace Aver3
{
    public class BTPrioritySelector : BTAction
    {
        protected BTAction m_activeChild;

        protected override bool OnEvaluate(BTData bTData)
        {
            for(int i = 0; i < childCount; i++)
            {
                var child = GetChild<BTAction>(i);
                if(child.Evaluate(bTData))
                {
                    m_activeChild = child;
                    return true;
                }
            }
            return false;
        }

        protected override BTResult OnUpdate(BTData bTData)
        {
            if(m_activeChild == null)
                return BTResult.Finished;

            var result = m_activeChild.Update(bTData);
            if(result != BTResult.Running)
                m_activeChild = null;

            return result;
        }
    }
}