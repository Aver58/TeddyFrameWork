namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Evaluates the specified conditional task. If the conditional task returns success then the child task is run and the child status is returned. If the conditional task does not " +
                     "return success then the child task is not run and a failure status is immediately returned.")]
    [TaskIcon("{SkinColor}ConditionalEvaluatorIcon.png")]
    public class ConditionalEvaluator : Decorator
    {
        [Tooltip("Should the conditional task be reevaluated every tick?")]
        public SharedBool reevaluate;
        [InspectTask]
        [Tooltip("The conditional task to evaluate")]
        public Conditional conditionalTask;
        [Tooltip("Should the inspected conditional task be labeled within the graph?")]
        public bool graphLabel;

        // The status of the child after it has finished running.
        private TaskStatus executionStatus = TaskStatus.Inactive;
        private bool checkConditionalTask = true;
        private bool conditionalTaskFailed = false;

        public override void OnAwake()
        {
            if (conditionalTask != null) {
                conditionalTask.Owner = Owner;
                conditionalTask.GameObject = gameObject;
                conditionalTask.Transform = transform;
                conditionalTask.OnAwake();
            }
        }

        public override void OnStart()
        {
            if (conditionalTask != null) {
                conditionalTask.OnStart();
            }
        }

        public override bool CanExecute()
        {
            // CanExecute is called when checking the condition within a while loop so it will be called at least twice. Ensure the conditional task is checked only once
            if (checkConditionalTask) {
                checkConditionalTask = false;
                OnUpdate();
            }

            if (conditionalTaskFailed) {
                return false;
            }
            return executionStatus == TaskStatus.Inactive || executionStatus == TaskStatus.Running;
        }

        public override bool CanReevaluate()
        {
            return reevaluate.Value;
        }

        public override TaskStatus OnUpdate()
        {
            var childStatus = conditionalTask.OnUpdate();
            conditionalTaskFailed = conditionalTask == null || childStatus == TaskStatus.Failure;
            return childStatus;
        }

        public override void OnChildExecuted(TaskStatus childStatus)
        {
            // Update the execution status after a child has finished running.
            executionStatus = childStatus;
        }

        public override TaskStatus OverrideStatus()
        {
            // This version of OverrideStatus is called when the conditional evaluator fails reevaluation and has to stop all of its children.
            // Therefore, the return status will always be failure
            return TaskStatus.Failure;
        }

        public override TaskStatus OverrideStatus(TaskStatus status)
        {
            if (conditionalTaskFailed)
                return TaskStatus.Failure;
            return status;
        }

        public override void OnEnd()
        {
            // Reset the variables back to their starting values.
            executionStatus = TaskStatus.Inactive;
            checkConditionalTask = true;
            conditionalTaskFailed = false;
            if (conditionalTask != null) {
                conditionalTask.OnEnd();
            }
        }

        public override string OnDrawNodeText()
        {
            if (conditionalTask == null || !graphLabel) {
                return string.Empty;
            }

            return conditionalTask.GetType().Name;
        }

        public override void OnReset()
        {
            // Reset the public properties back to their original values.
            conditionalTask = null;
        }
    }
}