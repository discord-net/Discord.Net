using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public interface ILoadableMessageActor<TMessage> :
    IMessageActor<TMessage>,
    ILoadableEntity<ulong, TMessage>
    where TMessage : class, IMessage;

public interface IMessageActor<out TMessage> :
    IMessageChannelRelationship, // IMessageChannelRelationship
    IModifiable<ulong, IMessageActor<TMessage>, ModifyMessageProperties, ModifyMessageParams>,
    IDeletable<ulong, IMessageActor<TMessage>>,
    IActor<ulong, TMessage>
    where TMessage : IMessage
{
    static BasicApiRoute IDeletable<ulong, IMessageActor<TMessage>>.DeleteRoute(IPathable path, ulong id)
        => Routes.DeleteMessage(path.Require<IChannel>(), id);

    static ApiBodyRoute<ModifyMessageParams> IModifiable<ulong, IMessageActor<TMessage>, ModifyMessageProperties, ModifyMessageParams>.ModifyRoute(IPathable path, ulong id, ModifyMessageParams args) =>
        Routes.ModifyMessage(path.Require<IChannel>(), id, args);
}
