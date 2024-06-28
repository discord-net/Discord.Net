using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

using IModifiable =
    IModifiable<ulong, IMessageActor, ModifyMessageProperties, ModifyMessageParams, IMessage, IMessageModel>;

public interface ILoadableMessageActor :
    IMessageActor,
    ILoadableEntity<ulong, IMessage>;

public interface IMessageActor :
    IMessageChannelRelationship,
    IModifiable,
    IDeletable<ulong, IMessageActor>,
    IActor<ulong, IMessage>
{
    static IApiRoute IDeletable<ulong, IMessageActor>.DeleteRoute(IPathable path, ulong id)
        => Routes.DeleteMessage(path.Require<IChannel>(), id);

    static IApiInOutRoute<ModifyMessageParams, IEntityModel> IModifiable.ModifyRoute(
        IPathable path,
        ulong id,
        ModifyMessageParams args
    ) => Routes.ModifyMessage(path.Require<IChannel>(), id, args);
}
