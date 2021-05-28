public class IdleRequest : BehaviorRequest
{
    public IdleRequest() : base(RequestType.Idle)
    {
        isRequestCompleted = true;
    }
}
