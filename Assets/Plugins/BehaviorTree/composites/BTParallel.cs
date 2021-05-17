namespace Aver3
{
    /// <summary>
    /// 并列节点
    /// 遍历所有节点，有and和or条件
    /// 如果是and条件，全部节点失败，结果才会失败
    /// 如果是or条件，任何一个节点失败，那么这次的结果就是失败
    /// </summary>
    public enum ParallelFunction
    {
        And = 1,    // returns Ended when all results are not running
        Or = 2,     // returns Ended when any result is not running
    }

    public class BTParallel : BTNode
    {
        private int m_failResultCount = 0;
        protected ParallelFunction m_func;

        public BTParallel(ParallelFunction func)
        {
            m_func = func;
        }

        protected override bool OnEvaluate()
        {
            bool isOrFunc = m_func == ParallelFunction.Or;
            for(int i = 0; i < childCount; i++)
            {
                var child = GetChild(i);
                if(isOrFunc && (child.Evaluate() == false))
                {
                    return false;
                }
            }
            return true;
        }

        protected override BTResult OnUpdate()
        {
            m_failResultCount = 0;
            bool isOrFunc = m_func == ParallelFunction.Or;
            for(int i = 0; i < childCount; i++)
            {
                var result = GetChild(i).Update();
                if(isOrFunc)
                {
                    
                }
                else
                {
                    if(result != BTResult.Running)
                    {
                        m_failResultCount += 1;
                    }
                }
            }

            if(m_failResultCount == childCount)
                return BTResult.Finished;

            return BTResult.Running;
        }
    }
}
