public class BehaviorRequest
{
    public RequestType RequestType;
    private bool m_isRequestCompleted;
    public Unit target;

    public BehaviorRequest(RequestType requestType)
    {
        m_isRequestCompleted = false;
        RequestType = requestType;
    }

    public void SetRequestCompleteState(bool isComplete)
    {
        m_isRequestCompleted = isComplete;
    }

    public bool IsRequestCompleted() 
    {
        return m_isRequestCompleted == true;
    }
}
