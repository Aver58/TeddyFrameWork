using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

// 混合 AnimationClip 和 AnimatorController
[RequireComponent(typeof(Animator))]
public class RuntimeControllerSample : MonoBehaviour {
    public AnimationClip clip;
    public RuntimeAnimatorController controller;
    public float weight;
    PlayableGraph playableGraph;
    AnimationMixerPlayable mixerPlayable;

    void Start() {
        // 创建该图和混合器，然后将它们绑定到 Animator。
        playableGraph = PlayableGraph.Create();
        var playableOutput = AnimationPlayableOutput.Create(playableGraph, "Animation", GetComponent<Animator>());
        mixerPlayable = AnimationMixerPlayable.Create(playableGraph, 2);
        playableOutput.SetSourcePlayable(mixerPlayable);

        // 创建 AnimationClipPlayable 并将它们连接到混合器。
        var clipPlayable = AnimationClipPlayable.Create(playableGraph, clip);
        var ctrlPlayable = AnimatorControllerPlayable.Create(playableGraph, controller);

        playableGraph.Connect(clipPlayable, 0, mixerPlayable, 0);
        playableGraph.Connect(ctrlPlayable, 0, mixerPlayable, 1);

        //播放该图。
        playableGraph.Play();
    }

    void Update() {
        weight = Mathf.Clamp01(weight);
        mixerPlayable.SetInputWeight(0, 1.0f - weight);
        mixerPlayable.SetInputWeight(1, weight);
    }

    void OnDisable() {
        //销毁该图创建的所有可播放项和输出。
        playableGraph.Destroy();
    }
}