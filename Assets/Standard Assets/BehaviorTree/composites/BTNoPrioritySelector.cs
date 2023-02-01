namespace Aver3
{
    /// <summary>
    /// 不优先选择节点
    /// 带记忆选择
    /// </summary>
    public class BTNoPrioritySelector : BTPrioritySelector
    {
        protected override bool OnEvaluate(BTData bTData)
        {
            if(m_activeChild != null)
            {
                if(m_activeChild.Evaluate(bTData))
                {
                    return true;
                }
            }
            return base.Evaluate(bTData);
        }
    }
}
