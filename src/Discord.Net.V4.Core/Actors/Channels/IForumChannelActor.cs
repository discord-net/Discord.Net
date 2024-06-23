using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public interface ILoadableForumChannelActor :
    IForumChannelActor,
    ILoadableEntity<ulong, IForumChannel>;

public interface IForumChannelActor :
    IThreadableGuildChannelActor,
    IActor<ulong, IForumChannel>,
    IModifiable<ulong, IForumChannelActor, ModifyForumChannelProperties, ModifyGuildChannelParams>
{
    static IApiInRoute<ModifyGuildChannelParams> IModifiable<ulong, IForumChannelActor, ModifyForumChannelProperties,
            ModifyGuildChannelParams>
        .ModifyRoute(IPathable path, ulong id, ModifyGuildChannelParams args)
        => Routes.ModifyChannel(id, args);
}
