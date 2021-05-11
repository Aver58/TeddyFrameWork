using BT;
using UnityEngine;

public class AITest : BTTree
{
    public float speed = 0.03f;
    public float sightForOrc = 2f;
    public float sightForGoblin = 1f;
    public float fightDistance = 0.2f;

    protected override void Init()
    {
        base.Init();

        _root = new BTPrioritySelector();// 父节点

        CheckTest check = new CheckTest();

        BTParallel run01 = new BTParallel(BTParallel.ParallelFunction.And, check);
        //Node_Run run11 = new Node_Run(speed);
        _root.AddChild(run01);
    }
}