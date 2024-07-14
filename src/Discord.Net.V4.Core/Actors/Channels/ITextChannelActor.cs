using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public interface ILoadableTextChannelActor :
    ITextChannelActor,
    ILoadableEntity<ulong, ITextChannel>;

[Modifiable<ModifyTextChannelProperties>(nameof(Routes.ModifyChannel))]
public partial interface ITextChannelActor :
    IMessageChannelActor,
    IThreadableChannelActor,
    IActor<ulong, ITextChannel>;
