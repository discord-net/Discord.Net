using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public interface ILoadableVoiceChannelActor :
    IVoiceChannelActor,
    ILoadableEntity<ulong, IVoiceChannel>;

[Modifiable<ModifyVoiceChannelProperties>(nameof(Routes.ModifyChannel))]
public partial interface IVoiceChannelActor :
    IMessageChannelActor,
    IGuildChannelActor,
    IActor<ulong, IVoiceChannel>;
