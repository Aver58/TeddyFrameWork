using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class TestConditional : Conditional {
    public float markCooldown = 5.0f; // 标记冷却时间（秒）
    private float markCooldownTimer = -1f;
    private float interval = 1;
    private float timer;
    private TaskStatus lastStatus;

    public override TaskStatus OnUpdate() {
        // if (Time.time - timer <= interval) {
        //     return lastStatus;
        // }

        // timer = Time.time;
        var status = IsSuccess();
        // if (lastStatus != status) {
        //     lastStatus = status;
        // }

        return status;
    }

    private TaskStatus IsSuccess() {
        if (markCooldownTimer != -1f && Time.time - markCooldownTimer <= markCooldown) {
            Debug.LogError($"还在冷却时间内，不能重复标记 {Time.time - markCooldownTimer}");
            return TaskStatus.Failure;
        }

        markCooldownTimer = Time.time;
        Debug.LogError($"markCooldownTimer {markCooldownTimer}");
        return TaskStatus.Success; // 成功标记
    }
}
