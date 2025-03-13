using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class TestAction : Action {
    [SerializeField] private SharedInt SharedInt;

    public override void OnStart() {
        base.OnStart();
        Debug.LogError("TestAction OnStart");
    }

    public override TaskStatus OnUpdate() {
        Debug.LogError("TestAction OnUpdate");
        return TaskStatus.Running;
    }

    public override void OnReset() {
        Debug.LogError("TestAction OnReset");
    }
}