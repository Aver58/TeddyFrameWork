using UnityEngine;
/// <summary>
/// 角色动画控制器管理器
/// </summary>
public class AnimationController : MonoBehaviour, IActor
{
    private Animator m_Animator;
    private string m_LastAnimName;

    private bool m_IsPause = false;

    //private bool m_IsHitStop = false;
    //private float m_HitStopTime = 0.3f;

    //private int m_DeadSinkTweenId = 0;

    public void Awake()
    {
        m_Animator = GetComponentInChildren<Animator>();
    }

    public void SetTrigger(string animName)
    { 
        if (!string.IsNullOrEmpty(m_LastAnimName)) {
            m_Animator.ResetTrigger(m_LastAnimName);
        }

        m_LastAnimName = animName;
        m_Animator.SetTrigger(animName);
    }

    public void SetTriggerWithSpeed(string animName, float speed)
    {
        m_Animator.speed = speed;
        SetTrigger(animName);
    }

    public void SetAnimSpeed(float speed)
    {
        m_Animator.speed = speed;
    }

    public void Continue()
    {
        m_IsPause = false;
        m_Animator.speed = 1.0f;

        //if(m_DeadSinkTweenId != 0)
        //{
        //    LeanTween.resume(m_DeadSinkTweenId);
        //}
    }

    public void Pause()
    {
        m_IsPause = true;
        m_Animator.speed = 0.0f;

        //if(m_DeadSinkTweenId != 0)
        //{
        //    LeanTween.pause(m_DeadSinkTweenId);
        //}
    }

    //private void Update()
    //{
    //    if (!m_IsPause)
    //    {
    //        if (m_IsHitStop)
    //        {
    //            m_HitStopTime = m_HitStopTime - Time.deltaTime;
    //            if (m_HitStopTime <= 0)
    //            {
    //                m_Animator.speed = 1f;
    //                m_IsHitStop = false;
    //            }
    //        }
    //    }
    //}

    //public void HitStop(float hitStopTime)
    //{
    //    m_Animator.speed = 0f;
    //    m_IsHitStop = true;
    //    m_HitStopTime = hitStopTime;
    //}

    //public void WaitDeadAnim(string animName, float sinkDelay, float sinkHeight, float sinkTime, Action deadAnimCompletedCallback)
    //{
    //    StartCoroutine(WaitDeadProgress(animName, sinkDelay, sinkHeight, sinkTime, deadAnimCompletedCallback));
    //}

    //public void StopDeadAnim()
    //{
    //    StopAllCoroutines();

    //    if(m_DeadSinkTweenId != 0)
    //    {
    //        LeanTween.cancel(m_DeadSinkTweenId);
    //        m_DeadSinkTweenId = 0;
    //    }
    //}

    //private IEnumerator WaitDeadProgress(string animName, float sinkDelay, float sinkHeight, float sinkTime, Action deadAnimCompletedCallback)
    //{
    //    yield return new WaitForAnimatorAnimEnd(m_Animator, animName);

    //    if (sinkDelay > 0)
    //    {
    //        float timePassed = 0f;
    //        while (timePassed < sinkDelay)
    //        {
    //            if (!m_IsPause) {
    //                timePassed += Time.deltaTime;
    //            }

    //            yield return null;
    //        }            
    //    }

    //    m_DeadSinkTweenId = LeanTween.moveLocalY(this.gameObject, sinkHeight, sinkTime).setOnComplete(() =>
    //    {
    //        if (deadAnimCompletedCallback != null) {
    //            deadAnimCompletedCallback();
    //        }
    //    }).id;
    //}
}
