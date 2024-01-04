using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Audio;
using UnityEngine.Playables;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class MultiOutputSample : MonoBehaviour {
    public AnimationClip animationClip;
    public AudioClip audioClip;
    PlayableGraph playableGraph;

    void Start() {
        playableGraph = PlayableGraph.Create();

        // 创建输出。
        var animationOutput = AnimationPlayableOutput.Create(playableGraph, "Animation", GetComponent<Animator>());
        var audioOutput = AudioPlayableOutput.Create(playableGraph, "Audio", GetComponent<AudioSource>());

        // 创建可播放项。
        var animationClipPlayable = AnimationClipPlayable.Create(playableGraph, animationClip);
        var audioClipPlayable = AudioClipPlayable.Create(playableGraph, audioClip, true);

        //将可播放项连接到输出
        animationOutput.SetSourcePlayable(animationClipPlayable);
        audioOutput.SetSourcePlayable(audioClipPlayable);

        // 播放该图。
        playableGraph.Play();
    }

    void OnDisable() {
        //销毁该图创建的所有可播放项和输出。
        playableGraph.Destroy();
    }
}