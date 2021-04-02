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
    /// 维护生命周期：OnEnter OnExit OnUpdate 
    /// </summary>
    public class BTAction : BTNode
    {
        public enum BTResult
        {
            Ready,
            Running,
            End,
        }

        private BTPrecondition m_precondition;
        public string Name { get { return GetType().ToString(); } }

        public void SetPrecondition(BTPrecondition precondition)
        {
            m_precondition = precondition;
        }

        public bool Evaluate()
        {
            // 评估这个节点是否可以进入：1.有设置条件；2.条件通过；
            if(m_precondition != null && m_precondition.IsTrue()) //&& OnEvaluate())
                return true;

            return false;
        }

        // 给子类提供个性化检查的接口
        //protected virtual bool OnEvaluate() { return true; }

        public BTResult Update()
        {
            return OnUpdate();
        }

        /// <summary>
        /// 第一次进入行为
        /// </summary>
        protected virtual void OnEnter() { }

        /// <summary>
        /// 离开行为
        /// </summary>
        protected virtual void OnExit() { }

        /// <summary>
        ///  行为的执行，返回BTResult
        /// </summary>
        protected virtual BTResult OnUpdate() { return BTResult.End; }
    }
}