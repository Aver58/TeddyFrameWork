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
    
    private bool _isTurning = false;
    private float _turnedTime = 0f;
    private Vector3 _lastForward;
    private Vector3 _currentForward;
    private Vector3 _targetForward;

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
                _isMoving = false;
        }

        //// Actor旋转
        //if (_isTurning)
        //{
        //    _turnedTime += Time.deltaTime;

        //    float radio = _turnedTime / _logicDeltaTime;
        //    _currentForward = Vector3.Lerp(_lastForward, _targetForward, radio);
        //    transform.forward = _currentForward;

        //    if (_turnedTime >= _logicDeltaTime)
        //    {
        //        _isTurning = false;
        //    }
        //}
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

    public void MoveTo2DPoint(float targetPosX, float targetPosZ, float forwardX, float forwardZ)
    {
        MoveTo3DPoint(targetPosX, transform.position.y, targetPosZ, forwardX, transform.forward.y, forwardZ);
    }

    public void MoveTo3DPoint(Vector3 targetPos)
    {
        _isMoving = true;
        _movedTime = 0f;
        _lastPos = _currentPos;
        _targetPos = targetPos;

        var forward = targetPos - _currentPos;
        transform.forward = forward;
    }

    public void MoveTo3DPoint(float targetPosX, float targetPosY, float targetPosZ, float forwardX, float forwardY, float forwardZ)
    {
        _isMoving = true;
        _movedTime = 0f;
        _lastPos = _currentPos;
        _targetPos = new Vector3(targetPosX, targetPosY, targetPosZ);
    
        transform.forward = new Vector3(forwardX, forwardY, forwardZ);
    }

    public void MoveTo3DPointY(float targetPosY)
    {
        var position = transform.position;
        var forward = transform.forward;
        MoveTo3DPoint(position.x, targetPosY, position.z, forward.x, forward.y, forward.z);
    }

    public void TurnTo2DForward(float forwardX, float forwardZ)
    {
        TurnTo3DForward(forwardX, transform.forward.y, forwardZ);
    }

    public void TurnTo3DForward(float forwardX, float forwardY, float forwardZ)
    {
        transform.forward = new Vector3(forwardX, forwardY, forwardZ);
    }

    public void BlinkTo2DPoint(float targetPosX, float targetPosZ, float forwardX, float forwardZ)
    {
        Set3DPosition(targetPosX, transform.position.y, targetPosZ);
        SetForward(forwardX, transform.forward.y, forwardZ);
    }

    public void SetPositionY(float posY)
    {
        var position = transform.position;
        Set3DPosition(position.x, posY, position.z);
    }

    private void Set3DPosition(Vector3 targetPos)
    {
        _isMoving = false;
        _lastPos = targetPos;
        _currentPos = _lastPos;
        transform.position = _currentPos;
    }

    private void Set3DPosition(float targetPosX, float targetPosY, float targetPosZ)
    {
        _isMoving = false;
        _lastPos = new Vector3(targetPosX, targetPosY, targetPosZ);
        _currentPos = _lastPos;
        transform.position = _currentPos;
    }

    private void SetForward(Vector3 forward)
    {
        _isTurning = false;
        _lastForward = forward;
        _currentForward = _lastForward;
        transform.forward = forward;
    }

    private void SetForward(float forwardX, float forwardY, float forwardZ)
    {
        _isTurning = false;
        _lastForward = new Vector3(forwardX, forwardY, forwardZ);
        _currentForward = _lastForward;
        transform.forward = new Vector3(forwardX, forwardY, forwardZ);
    }
}