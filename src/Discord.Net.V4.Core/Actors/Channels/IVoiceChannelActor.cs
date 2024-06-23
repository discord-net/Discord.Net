using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public interface ILoadableVoiceChannelActor :
    IVoiceChannelActor,
    ILoadableEntity<ulong, IVoiceChannel>;

public interface IVoiceChannelActor :
    ITextChannelActor,
    IModifiable<ulong, IVoiceChannelActor, ModifyVoiceChannelProperties, ModifyGuildChannelParams>,
    IActor<ulong, IVoiceChannel>
{
    static IApiInRoute<ModifyGuildChannelParams>
        IModifiable<ulong, IVoiceChannelActor, ModifyVoiceChannelProperties, ModifyGuildChannelParams>.ModifyRoute(
            IPathable path, ulong id, ModifyGuildChannelParams args)
        => Routes.ModifyChannel(id, args);
}
