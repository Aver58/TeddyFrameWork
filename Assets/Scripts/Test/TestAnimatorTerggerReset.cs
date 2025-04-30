using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAnimatorTerggerReset : MonoBehaviour
{
    public Animator animator;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // 假设空格为跳跃键
        {
            animator.SetTrigger("JumpTrigger");
            Debug.Log("JumpTrigger set");
        }
    }
}
