namespace TsiU
{
    public class TBTActionPrioritizedSelector : TBTAction
    {
        protected class TBTActionPrioritizedSelectorContext : TBTActionContext
        {
            internal int currentSelectedIndex;
            internal int lastSelectedIndex;

            public TBTActionPrioritizedSelectorContext()
            {
                currentSelectedIndex = -1;
                lastSelectedIndex = -1;
            }
        }

        public TBTActionPrioritizedSelector(): base(-1){}

        protected override bool onEvaluate(/*in*/TBTWorkingData wData)
        {
            var thisContext = getContext<TBTActionPrioritizedSelectorContext>(wData);
            thisContext.currentSelectedIndex = -1;
            int childCount = GetChildCount();
            //从左到右遍历自己的子节点
            for(int i = 0; i < childCount; ++i) 
            {
                var node = GetChild<TBTAction>(i);
                //如果子节点的准入条件符合信息的话，就执行该子节点。
                if (node.Evaluate(wData)) 
                {
                    thisContext.currentSelectedIndex = i;
                    return true;
                }
            }
            return false;
        }

        protected override int onUpdate(TBTWorkingData wData)
        {
            var thisContext = getContext<TBTActionPrioritizedSelectorContext>(wData);
            int runningState = TBTRunningStatus.FINISHED;
            //action没执行完毕，前提就变了，所以这边需要进行节点的转换来清除上个节点的状态
            if(thisContext.currentSelectedIndex != thisContext.lastSelectedIndex) 
            {
                if (IsIndexValid(thisContext.lastSelectedIndex)) 
                {
                    var node = GetChild<TBTAction>(thisContext.lastSelectedIndex);
                    node.Transition(wData);
                }
                thisContext.lastSelectedIndex = thisContext.currentSelectedIndex;
            }
            // Update
            if(IsIndexValid(thisContext.lastSelectedIndex)) 
            {
                var node = GetChild<TBTAction>(thisContext.lastSelectedIndex);
                runningState = node.Update(wData);
                //action执行完毕，那么下一次tick不需要进行节点转换
                if(TBTRunningStatus.IsFinished(runningState)) 
                    thisContext.lastSelectedIndex = -1;
            }
            return runningState;
        }
        protected override void onTransition(TBTWorkingData wData)
        {
            var thisContext = getContext<TBTActionPrioritizedSelectorContext>(wData);
            var node = GetChild<TBTAction>(thisContext.lastSelectedIndex);
            if (node != null) 
                node.Transition(wData);

            thisContext.lastSelectedIndex = -1;
        }
    }
}
