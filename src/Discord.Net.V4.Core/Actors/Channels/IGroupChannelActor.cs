using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public interface ILoadableGroupChannelActor :
    IGroupChannelActor,
    ILoadableEntity<ulong, IGroupChannel>;

[Modifiable<ModifyGroupDMProperties>(nameof(Routes.ModifyChannel))]
public partial interface IGroupChannelActor :
    IMessageChannelActor,
    IActor<ulong, IGroupChannel>;
