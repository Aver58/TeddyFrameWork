using UnityEngine;

public static class AnimatorExtends
{
    public static bool IsPlaying(this Animator anim, string stateName)
    {
        var stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        //animator所处的state播放完毕，如果未进入下一个state，
        //且没有循环，则该state的normalizedtime会大于1（state末尾）或小于0（负数，倒播到开头处，speed为负数）；
        if(stateInfo.IsName(stateName) && stateInfo.normalizedTime < 1.0f)
            return true;
        else
            return false;
    }

    public static bool IsInState(this Animator anim, string stateName)
    {
        var stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(stateName);
    }

    public static bool HaveClip(this Animator anim, string clipName)
    {
        var controller = anim.runtimeAnimatorController;
        if(controller!=null)
        {
            var animationClips = controller.animationClips;
            for(int i = 0; i < animationClips.Length; i++)
            {
                if(animationClips[i].name == clipName)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
