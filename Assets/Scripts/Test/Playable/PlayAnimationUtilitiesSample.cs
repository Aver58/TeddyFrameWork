using UnityEngine;
using UnityEngine.Playables;

// 使用 AnimationPlayableUtilities 来简化动画可播放项的创建和播放
[RequireComponent(typeof(Animator))]
public class PlayAnimationUtilitiesSample : MonoBehaviour {
    public AnimationClip clip;
    PlayableGraph playableGraph;

    void Start() {
        AnimationPlayableUtilities.PlayClip(GetComponent<Animator>(), clip, out playableGraph);
    }

    void OnDisable() {
        // 销毁该图创建的所有可播放项和输出。
        playableGraph.Destroy();
    }
}