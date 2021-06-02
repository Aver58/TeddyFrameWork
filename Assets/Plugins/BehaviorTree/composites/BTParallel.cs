using System.Collections.Generic;

namespace Aver3
{
    /// <summary>
    /// 并列节点
    /// 遍历所有节点，有and和or条件
    /// </summary>

    public enum ParallelFunction
    {
        And = 1,    // 如果是and条件，任何一个节点失败，那么这次的结果就是失败
        Or = 2,     // 如果是or条件，全部节点失败，结果才会失败
    }

    public class BTParallel : BTAction
    {
        private int m_failResultCount = 0;
        protected ParallelFunction m_func;
        protected List<BTResult> m_results;

        public BTParallel(ParallelFunction func)
        {
            m_results = new List<BTResult>();
            m_func = func;
        }

        protected override bool OnEvaluate(BTData bTData)
        {
            bool isAndFunc = m_func == ParallelFunction.And;
            for(int i = 0; i < childCount; i++)
            {
                var child = GetChild<BTAction>(i);
                if(isAndFunc && (child.Evaluate(bTData) == false))
                {
                    return false;
                }
            }
            return true;
        }

        protected override BTResult OnUpdate(BTData bTData)
        {
            // todo
            //m_failResultCount = 0;
            //bool isAndFunc = m_func == ParallelFunction.And;
            //for(int i = 0; i < childCount; i++)
            //{
            //    var result = GetChild(i).Update();
            //    if(isAndFunc)
            //    {
                    
            //    }
            //    else
            //    {
            //        if(result != BTResult.Running)
            //        {
            //            m_failResultCount += 1;
            //        }
            //    }
            //}

            //if(m_failResultCount == childCount)
            //    return BTResult.Finished;

            return BTResult.Running;
        }
    }
}
