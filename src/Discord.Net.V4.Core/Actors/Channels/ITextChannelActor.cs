using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public interface ILoadableTextChannelActor :
    ITextChannelActor,
    ILoadableEntity<ulong, ITextChannel>;

public interface ITextChannelActor :
    IMessageChannelActor,
    IThreadableGuildChannelActor,
    IActor<ulong, ITextChannel>,
    IModifiable<ulong, ITextChannelActor, ModifyTextChannelProperties, ModifyGuildChannelParams>
{
    static ApiBodyRoute<ModifyGuildChannelParams> IModifiable<ulong, ITextChannelActor, ModifyTextChannelProperties, ModifyGuildChannelParams>.ModifyRoute(IPathable path, ulong id,
        ModifyGuildChannelParams args)
        => Routes.ModifyChannel(id, args);
}
