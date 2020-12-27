public class AIBehaviorRequest
{
    public RequestType RequestType { get; }
    public bool isRequestCompleted { get; set; }
    public Entity target { get; set; }
    public AIBehaviorRequest(RequestType requestType)
    {
        isRequestCompleted = false;
        RequestType = requestType;
    }
}
