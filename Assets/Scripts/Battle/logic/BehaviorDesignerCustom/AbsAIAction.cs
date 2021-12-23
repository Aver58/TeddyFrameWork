using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class AbsAIAction : Action
{
    protected Transform role;

    public override void OnStart() {
        Init();
    }

    public override void OnReset() {
        Clear();
    }

    public override void OnBehaviorComplete() {
        Clear();
    }

    private void Clear() {
        
    }

    private void Init() {
        
    }
}