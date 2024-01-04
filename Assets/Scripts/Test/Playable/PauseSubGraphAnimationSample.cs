using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

// 控制树的播放状态
[RequireComponent(typeof(Animator))]
public class PauseSubGraphAnimationSample : MonoBehaviour {
    public AnimationClip clip0;
    public AnimationClip clip1;
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

        mixerPlayable.SetInputWeight(0, 1.0f);
        mixerPlayable.SetInputWeight(1, 1.0f);
        clipPlayable1.Pause();

        //播放该图。
        playableGraph.Play();
    }

    void OnDisable() {
        //销毁该图创建的所有可播放项和输出。
        playableGraph.Destroy();
    }
}