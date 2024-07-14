using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public interface ILoadableForumChannelActor :
    IForumChannelActor,
    ILoadableEntity<ulong, IForumChannel>;

[Modifiable<ModifyForumChannelProperties>(nameof(Routes.ModifyChannel))]
public partial interface IForumChannelActor :
    IThreadableChannelActor,
    IActor<ulong, IForumChannel>;
