using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public interface ILoadableMessageActor :
    IMessageActor,
    ILoadableEntity<ulong, IMessage>;

public interface IMessageActor :
    IMessageChannelRelationship,
    IModifiable<ulong, IMessageActor, ModifyMessageProperties, ModifyMessageParams>,
    IDeletable<ulong, IMessageActor>,
    IActor<ulong, IMessage>
{
    static IApiRoute IDeletable<ulong, IMessageActor>.DeleteRoute(IPathable path, ulong id)
        => Routes.DeleteMessage(path.Require<IChannel>(), id);

    static IApiInRoute<ModifyMessageParams>
        IModifiable<ulong, IMessageActor, ModifyMessageProperties, ModifyMessageParams>.ModifyRoute(IPathable path,
            ulong id, ModifyMessageParams args) =>
        Routes.ModifyMessage(path.Require<IChannel>(), id, args);
}
