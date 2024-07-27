namespace Discord.Gateway.Events;

public interface IGatewayDispatch
{
    string EventName { get; }
}
