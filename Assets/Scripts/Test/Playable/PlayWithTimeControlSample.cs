using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

// 控制树的时序
[RequireComponent(typeof(Animator))]
public class PlayWithTimeControlSample : MonoBehaviour {
    public AnimationClip clip;
    public float time;
    PlayableGraph playableGraph;
    AnimationClipPlayable playableClip;

    void Start() {
        playableGraph = PlayableGraph.Create();
        var playableOutput = AnimationPlayableOutput.Create(playableGraph, "Animation", GetComponent<Animator>());

        // 将剪辑包裹在可播放项中
        playableClip = AnimationClipPlayable.Create(playableGraph, clip);

        // 将可播放项连接到输出
        playableOutput.SetSourcePlayable(playableClip);

        // 播放该图。
        playableGraph.Play();

        //使时间停止自动前进。
        playableClip.SetPlayState(PlayState.Paused);
    }

    void Update() {
        //手动控制时间
        playableClip.SetTime(time);
    }


    void OnDisable() {
        // 销毁该图创建的所有可播放项和输出。
        playableGraph.Destroy();
    }
}