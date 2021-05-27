using UnityEngine;

public static class AnimatorExtends
{
    public static bool isPlaying(this Animator anim, string stateName)
    {
        if(anim.GetCurrentAnimatorStateInfo(0).IsName(stateName) 
            && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            return true;
        else
            return false;
    }
}
