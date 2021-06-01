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
        private BTActionStatus m_status = BTActionStatus.Ready;
        public string Name { get { return GetType().ToString(); } }

        /// <summary>
        ///  行为的执行，返回BTResult
        /// </summary>
        protected override BTResult OnUpdate(BTData bTData)
        {
            BTResult result = BTResult.Finished;
            if(m_status == BTActionStatus.Ready)
            {
                m_status = BTActionStatus.Running;
                OnEnter(bTData);
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

        /// <summary>
        /// 第一次进入行为
        /// </summary>
        protected virtual void OnEnter(BTData bTData) 
        {
            if(BTConst.ENABLE_BTACTION_LOG)
                GameLog.Log("OnEnter " + " [" + this.Name + "]");
        }

        /// <summary>
        /// 行为进行中
        /// </summary>
        /// <returns></returns>
        protected virtual BTResult OnExecute(BTData bTData)
        {
            if(BTConst.ENABLE_BTACTION_LOG)
                GameLog.Log("OnExecute " + " [" + this.Name + "]");

            return BTResult.Finished;
        }

        /// <summary>
        /// 离开行为
        /// </summary>
        protected virtual void OnExit(BTData bTData) 
        {
            if(BTConst.ENABLE_BTACTION_LOG)
                GameLog.Log("OnExit " + " [" + this.Name + "]");
        }
    }
}