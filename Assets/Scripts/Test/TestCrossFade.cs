using UnityEngine;

public class TestCrossFade : MonoBehaviour
{
    public Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim.CrossFade("A2", 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        if(anim.GetCurrentAnimatorStateInfo(0).IsName("A2"))
        {
            GameLog.RawLog(Time.time);
            enabled = false;
        }
    }
}
