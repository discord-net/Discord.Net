using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public interface ILoadableMessageActor<TMessage> :
    IMessageActor,
    ILoadableEntity<ulong, TMessage>
    where TMessage : class, IMessage;

public interface IMessageActor :
    IMessageChannelRelationship,
    IModifiable<ulong, IMessageActor, ModifyMessageProperties, ModifyMessageParams>,
    IDeletable<ulong, IMessageActor>,
    IActor<ulong, IMessage>
{
    static BasicApiRoute IDeletable<ulong, IMessageActor>.DeleteRoute(IPathable path, ulong id)
        => Routes.DeleteMessage(path.Require<IChannel>(), id);

    static ApiBodyRoute<ModifyMessageParams> IModifiable<ulong, IMessageActor, ModifyMessageProperties, ModifyMessageParams>.ModifyRoute(IPathable path, ulong id, ModifyMessageParams args) =>
        Routes.ModifyMessage(path.Require<IChannel>(), id, args);
}
