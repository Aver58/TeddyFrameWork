using UnityEngine;

public class TestAnimatorOverrideController : MonoBehaviour {
    public Animator Animator;
    public AnimationClip AnimationClip;
    void Start() {

    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            // 测试
            var overrideController = new AnimatorOverrideController(Animator.runtimeAnimatorController);
            overrideController.name = "new overrideController";
            overrideController["vertical"] = AnimationClip;
            Animator.runtimeAnimatorController = overrideController;
        }
    }
}