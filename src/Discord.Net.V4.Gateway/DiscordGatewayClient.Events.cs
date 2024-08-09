using Discord.Gateway.Dispatch;
using System.Diagnostics.CodeAnalysis;

namespace Discord.Gateway;

public sealed partial class DiscordGatewayClient
{
    // public UserUpdatedEvent UserUpdatedEvent { get; private set; }
    // public event UserUpdatedDelegate UserUpdated
    // {
    //     add => UserUpdatedEvent.Subscribe(value);
    //     remove => UserUpdatedEvent.Unsubscribe(value);
    // }
    //
    // [MemberNotNull(nameof(UserUpdatedEvent))]
    // private void InitializeEvents()
    // {
    //     UserUpdatedEvent ??= new(this);
    // }

    public IEventDispatcher GetDispatcher(string eventName)
        => Config.EventDispatchers.TryGetValue(eventName, out var dispatcher)
            ? dispatcher
            : Config.DefaultEventDispatcher.Get(this);
}
