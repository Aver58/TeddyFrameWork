public abstract class State
{
    protected abstract void OnEnter();
    protected abstract void OnExit();
    public void OnUpdate(float dt){ }
}
