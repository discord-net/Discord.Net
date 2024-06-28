using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

using IModifiable =
    IModifiable<ulong, IVoiceChannelActor, ModifyVoiceChannelProperties, ModifyGuildChannelParams, IVoiceChannel,
        IGuildVoiceChannelModel>;

public interface ILoadableVoiceChannelActor :
    IVoiceChannelActor,
    ILoadableEntity<ulong, IVoiceChannel>;

public interface IVoiceChannelActor :
    ITextChannelActor,
    IModifiable,
    IActor<ulong, IVoiceChannel>
{
    static IApiInOutRoute<ModifyGuildChannelParams, IEntityModel> IModifiable.ModifyRoute(
        IPathable path,
        ulong id,
        ModifyGuildChannelParams args
    ) => Routes.ModifyChannel(id, args);
}
