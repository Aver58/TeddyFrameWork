#region Copyright © 2018 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    Node_Run.cs
 Author:      Zeng Zhiwei
 Time:        2019/12/2 11:29:38
=====================================================
*/
#endregion

using BT;
using UnityEngine;

public class Node_Run: BTAction
{
    private float _speed;
    private Transform _trans;
    private int _destinationDataId;
    private string _destinationDataName;
    private Vector3 _destination;
    private float _tolerance = 0.01f;

    public Node_Run(string destinationDataName, float speed)
    {
        _destinationDataName = destinationDataName;
        _destinationDataId = database.GetDataId(_destinationDataName);
        _trans = database.transform;
        _speed = speed;
    }

    protected override BTResult Execute()
    {
        _destination = database.GetData<Vector3>(_destinationDataId);
        UpdateFaceDirection();
        if (CheckArrived())
            return BTResult.Ended;

        MoveToDestination();
        return BTResult.Running;
    }

    private void MoveToDestination()
    {
        Vector3 direction = (_destination - _trans.position).normalized;
        _trans.position += direction * _speed;
    }

    private void UpdateFaceDirection()
    {
        Vector3 offset = _destination - _trans.position;
        _trans.localEulerAngles = offset.x >= 0 ? new Vector3(0, 180, 0) : Vector3.zero;
    }

    private bool CheckArrived()
    {
        Vector3 offset = _destination - _trans.position;
        return offset.sqrMagnitude < _tolerance * _tolerance;
    }
}

