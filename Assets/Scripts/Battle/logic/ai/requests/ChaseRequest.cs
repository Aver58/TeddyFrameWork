#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    ChaseRequest.cs
 Author:      Zeng Zhiwei
 Time:        2020/5/17 13:21:20
=====================================================
*/
#endregion

public class ChaseRequest : BehaviorRequest
{
    public ChaseRequest(Unit target) : base(RequestType.Chase)
    {
        this.target = target;
        isRequestCompleted = false;
    }
}