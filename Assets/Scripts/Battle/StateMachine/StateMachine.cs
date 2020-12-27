public class StateMachine
{
    private State _curState;

    public void Update(float dt)
    {
        if(_curState!=null)
        {
            _curState.OnUpdate(dt);
        }
    }
}