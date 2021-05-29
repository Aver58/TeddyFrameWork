public class BehaviorRequest
{
    public RequestType RequestType { get; }
    public bool isRequestCompleted { get; set; }
    public Unit target { get; set; }

    public BehaviorRequest(RequestType requestType)
    {
        isRequestCompleted = false;
        RequestType = requestType;
    }

    public bool IsRequestCompleted() 
    {
        return isRequestCompleted == true;
    }
}
