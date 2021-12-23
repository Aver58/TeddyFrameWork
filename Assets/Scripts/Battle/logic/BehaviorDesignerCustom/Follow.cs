using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Battle.logic.BehaviorDesignerCustom {
    // private SharedRoleLogic followTarget;
    
    public class Follow : AbsAIAction {
        public override TaskStatus OnUpdate() {
            // transform.position = target
            return TaskStatus.Success;
        }
    }
}