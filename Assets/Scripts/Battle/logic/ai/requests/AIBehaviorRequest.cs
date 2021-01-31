public class AIBehaviorRequest
{
    public RequestType RequestType { get; }
    public bool isRequestCompleted { get; set; }
    public Unit target { get; set; }

    public AIBehaviorRequest(RequestType requestType)
    {
        isRequestCompleted = false;
        RequestType = requestType;
    }

    public bool IsRequestCompleted() 
    {
        return isRequestCompleted == true;
    }
}
