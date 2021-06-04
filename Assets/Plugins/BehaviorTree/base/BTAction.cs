#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    BTAction.cs
 Author:      Zeng Zhiwei
 Time:        2021/4/2 11:03:19
=====================================================
*/
#endregion

namespace Aver3
{
    /// <summary>
    /// BTAction是负责游戏逻辑的行为节点，也就是行为树里面的“行为”。
    /// 外部调用Evaluate来判断节点是否可以进入，可以进入的话，调用Update来获取执行结果
    /// 维护生命周期：OnEnter OnExit OnExecute 
    /// </summary>
    public class BTAction : BTNode
    {
        private BTPrecondition m_precondition;
        private BTActionStatus m_status = BTActionStatus.Ready;
        protected virtual bool OnEvaluate(BTData bTData) { return true; }

        public string Name { get { return GetType().ToString(); } }

        #region API

        public bool Evaluate(BTData bTData)
        {
            // 评估这个节点是否可以进入：1.有设置条件；2.条件通过；
            var condition1 = (m_precondition == null || m_precondition.IsTrue(bTData));
            var condition2 = OnEvaluate(bTData);
            return condition1 && condition2;
        }

        public BTResult Update(BTData bTData)
        {
            return OnUpdate(bTData);
        }

        public void SetPrecondition(BTPrecondition precondition)
        {
            m_precondition = precondition;
        }

        #endregion

        #region 叶子节点，生命周期维护

        /// <summary>
        ///  行为的生命周期维护，返回BTResult
        /// </summary>
        protected virtual BTResult OnUpdate(BTData bTData)
        {
            BTResult result = BTResult.Finished;
            if(m_status == BTActionStatus.Ready)
            {
                OnEnter(bTData);
                m_status = BTActionStatus.Running;
            }

            if(m_status == BTActionStatus.Running)
            {
                result = OnExecute(bTData);
                if(result != BTResult.Running)
                {
                    OnExit(bTData);
                    m_status = BTActionStatus.Ready;
                }
            }

            return result;
        }

        protected virtual void OnEnter(BTData bTData) 
        {
            if(BTConst.ENABLE_BTACTION_LOG)
                GameLog.Log("OnEnter【{0}】", Name);
        }
        protected virtual BTResult OnExecute(BTData bTData) 
        {
            if(BTConst.ENABLE_BTACTION_LOG)
                GameLog.Log("OnExecute【{0}】", Name);
            return BTResult.Finished; 
        }
        protected virtual void OnExit(BTData bTData) 
        {
            if(BTConst.ENABLE_BTACTION_LOG)
                GameLog.Log("OnExit【{0}】", Name);
        }

        #endregion
    }
}