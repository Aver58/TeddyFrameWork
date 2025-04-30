using UnityEngine;

public class TestAnimatorOverrideController : MonoBehaviour {
    public Animator Animator;
    public AnimationClip AnimationClip;
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            var overrideController = new AnimatorOverrideController(Animator.runtimeAnimatorController);
            overrideController.name = "new overrideController";
            overrideController["vertical"] = AnimationClip;
            Animator.runtimeAnimatorController = overrideController;
        }
    }
}