using Discord.Gateway.Events.Processors;
using Discord.Models;

namespace Discord.Gateway.Dispatch;

file sealed class UserUpdatedEventPackage
{

}

public delegate ValueTask UserUpdatedDelegate(GatewayUser user);

public sealed class UserUpdatedEvent :
    IGatewayEvent<UserUpdatedDelegate>,
    IDispatchProcessor<IUserModel>
{
    public IReadOnlyCollection<IInvocableEventHandler> Handlers => throw new NotImplementedException();



    public void Subscribe(UserUpdatedDelegate handler) => throw new NotImplementedException();

    public void Unsubscribe(UserUpdatedDelegate handler) => throw new NotImplementedException();

    // userid
    // user actor
    // IEntityHandle<ulong, GatewayUser>
    // GatewayUser
}
