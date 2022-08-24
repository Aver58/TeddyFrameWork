#region Copyright © 2018 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    CheckTest.cs
 Author:      Zeng Zhiwei
 Time:        2019/12/2 11:49:06
=====================================================
*/
#endregion

using BT;
using UnityEngine;

public class CheckTest : BTPrecondition
{
    public override bool Check()
    {
        int random = Random.Range(0, 2);

        //Debug.Log("CheckTest: "+ (random == 0).ToString());
        return true;
    }
}

