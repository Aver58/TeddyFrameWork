#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    TestBT.cs
 Author:      Zeng Zhiwei
 Time:        2021/4/2 16:27:52
=====================================================
*/
#endregion

using Aver3;
using UnityEngine;

public class TestBT : MonoBehaviour 
{
    private BTAction root;
    void Awake()
    {
        // 构建行为树
        root = new BTPrioritySelector();
        root.AddChild(new ActionPatrol());
        root.AddChild(new ActionIdle());
    }

    private void Update()
    {
        // 开始测试
        //for(int i = 0; i < root.childCount; i++)
        //{
        //    var node = root.GetChild(i) as BTAction;
        //    if(node.Evaluate())
        //    {
        //        node.Update();
        //        break;
        //    }
        //}

        if(root.Evaluate())
            root.Update();
    }
}