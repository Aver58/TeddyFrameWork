public class IdleRequest : AIBehaviorRequest
{
    public IdleRequest() : base(RequestType.Idle)
    {
        isRequestCompleted = true;
    }
}
