namespace Aver3
{
    /// <summary>
    /// 序列节点
    /// todo test
    /// 将其所有子节点依次执行，也就是说当前一个返回“完成”状态后，再运行下一个子节点
    /// </summary>
    public class BTSequence : BTAction
    {
        private int m_activeIndex = 0;

        protected override bool OnEvaluate(BTData bTData)
        {
            if(m_activeIndex == -1)
                m_activeIndex = 0;

            if(m_activeIndex != -1)
            {
                var child = GetChild<BTAction>(m_activeIndex);
                bool result = child.Evaluate(bTData);
                if(result == false)
                {
                    m_activeIndex = -1;
                    return false;
                }
            }

            return GetChild<BTAction>(0).Evaluate(bTData);
        }

        protected override BTResult OnUpdate(BTData bTData)
        {
            var child = GetChild<BTAction>(m_activeIndex);
            var result = child.Update(bTData);
            if(result == BTResult.Finished)
            {
                m_activeIndex++;
                if(m_activeIndex > childCount)
                {
                    m_activeIndex = -1;
                }
                else
                {
                    result = BTResult.Running;
                }
            }
            return result;
        }
    }
}