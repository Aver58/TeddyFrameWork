using System;
using UnityEngine;

public class PositionController : MonoBehaviour, IActor
{
    private bool _isPause = false;
    private float _logicDeltaTime = 0.2f;   // 逻辑间隔时间

    private bool _isMoving = false;
    private float _movedTime = 0f;

    private Vector3 _lastPos;               // 上一次移动后的位置
    private Vector3 _currentPos;            // 当前位置
    private Vector3 _targetPos;             // 目标位置
    
    private Vector3 _lastForward;
    private Vector3 _currentForward;
    private Vector3 _targetForward;
    private event Action m_OnMoveEnd;

    private void LateUpdate()
    {
        if (_isPause) 
            return;

        // 移动Actor，视图的更新应该比逻辑的快，假设逻辑更新为1s一次，移动了多少距离，视图这边需要差值来进行平缓的移动
        if (_isMoving)
        {
            _movedTime += Time.deltaTime;

            float radio = _movedTime / _logicDeltaTime;
            _currentPos = Vector3.Lerp(_lastPos, _targetPos, radio);
            transform.position = _currentPos;

            if (_movedTime >= _logicDeltaTime)
            {
                _isMoving = false;
                if(m_OnMoveEnd != null)
                    m_OnMoveEnd();
            }
        }
    }

    public void Continue()
    {
        _isPause = false;
    }

    public void Pause()
    {
        _isPause = true;
    }

    public void InitPosition(Vector3 positon, Vector3 forward)
    {
        Set3DPosition(positon);
        SetForward(forward);
    }

    public void MoveTo3DPoint(float deltaTime,Vector3 targetPos, Action callback)
    {
        _isMoving = true;
        _movedTime = 0f;
        _logicDeltaTime = deltaTime;
        _lastPos = _currentPos;
        _targetPos = targetPos;

        var forward = targetPos - _currentPos;
        transform.forward = forward;
        m_OnMoveEnd = callback;
    }

    private void Set3DPosition(Vector3 targetPos)
    {
        _isMoving = false;
        _lastPos = targetPos;
        _currentPos = _lastPos;
        transform.position = _currentPos;
    }

    private void SetForward(Vector3 forward)
    {
        _lastForward = forward;
        _currentForward = _lastForward;
        transform.forward = forward;
    }
}