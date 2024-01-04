using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

// 创建动画混合树
[RequireComponent(typeof(Animator))]
public class MixAnimationSample : MonoBehaviour {
    public AnimationClip clip0;
    public AnimationClip clip1;
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
        var clipPlayable0 = AnimationClipPlayable.Create(playableGraph, clip0);
        var clipPlayable1 = AnimationClipPlayable.Create(playableGraph, clip1);

        playableGraph.Connect(clipPlayable0, 0, mixerPlayable, 0);
        playableGraph.Connect(clipPlayable1, 0, mixerPlayable, 1);

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