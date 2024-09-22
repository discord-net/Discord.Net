using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

[
    Loadable(nameof(Routes.GetChannel), typeof(GuildForumChannel)),
    Modifiable<ModifyForumChannelProperties>(nameof(Routes.ModifyChannel)),
    Creatable<CreateGuildForumChannelProperties>(
        nameof(Routes.CreateGuildChannel),
        nameof(IGuildActor),
        RouteGenerics = [typeof(GuildForumChannel)]
    ), 
    BackLink(nameof(Threads)),
    SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")
]
public partial interface IForumChannelActor :
    IThreadableChannelActor,
    IIntegrationChannelTrait.WithIncoming,
    IActor<ulong, IForumChannel>;