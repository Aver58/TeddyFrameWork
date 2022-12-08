using UnityEngine;
 
public class TestAnimation : MonoBehaviour {
 
    [Range(0.0f, 1.0f)]
    public float slider;
 
    private Animation animationComponent;    
    private AnimationClip animationClip;
 
    void Start () {
 
        animationComponent = GetComponent<Animation>();
        animationComponent.enabled = true;
        animationClip = animationComponent.clip;
        animationComponent[animationClip.name].weight = 1;
        animationComponent.Play();
        animationComponent[animationClip.name].speed = 0;
    }
 
    void Update () {
 
        animationComponent[animationClip.name].normalizedTime = slider;
    }
}
