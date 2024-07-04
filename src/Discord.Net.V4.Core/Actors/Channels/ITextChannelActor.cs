using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

using IModifiable =
    IModifiable<ulong, ITextChannelActor, ModifyTextChannelProperties, ModifyGuildChannelParams, ITextChannel,
        IGuildTextChannelModel>;

public interface ILoadableTextChannelActor :
    ITextChannelActor,
    ILoadableEntity<ulong, ITextChannel>;

public interface ITextChannelActor :
    IMessageChannelActor,
    IThreadableChannelActor,
    IActor<ulong, ITextChannel>,
    IModifiable
{
    static IApiInOutRoute<ModifyGuildChannelParams, IEntityModel>
        IModifiable.ModifyRoute(
            IPathable path, ulong id,
            ModifyGuildChannelParams args)
        => Routes.ModifyChannel(id, args);
}
