namespace Discord.Gateway;

public interface IGatewayEvent<in T> where T : Delegate
{
    IReadOnlyCollection<IInvocableEventHandler> Handlers { get; }

    void Subscribe(T handler);
    void Unsubscribe(T handler);
}
