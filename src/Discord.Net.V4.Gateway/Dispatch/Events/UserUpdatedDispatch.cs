namespace Discord.Gateway.Events.Events;

public sealed class UserUpdatedDispatch : IGatewayEvent<Func<IEntityHandle<ulong, GatewayUser>, ValueTask>>
{
    public IReadOnlyCollection<IInvocableEventHandler> Handlers => throw new NotImplementedException();



    public void Subscribe(Func<IEntityHandle<ulong, GatewayUser>, ValueTask> handler) => throw new NotImplementedException();

    public void Unsubscribe(Func<IEntityHandle<ulong, GatewayUser>, ValueTask> handler) => throw new NotImplementedException();
}
