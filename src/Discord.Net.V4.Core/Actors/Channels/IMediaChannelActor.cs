using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public interface ILoadableMediaChannelActor :
    IMediaChannelActor,
    ILoadableEntity<ulong, IMediaChannel>;

[Modifiable<ModifyMediaChannelProperties>(nameof(Routes.ModifyChannel))]
public partial interface IMediaChannelActor :
    IThreadableChannelActor,
    IActor<ulong, IMediaChannel>;
