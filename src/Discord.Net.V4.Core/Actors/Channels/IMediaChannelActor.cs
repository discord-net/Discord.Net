using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

[
    Loadable(nameof(Routes.GetChannel), typeof(GuildMediaChannel)),
    Modifiable<ModifyMediaChannelProperties>(nameof(Routes.ModifyChannel)),
    Creatable<CreateGuildMediaChannelProperties>(
        nameof(Routes.CreateGuildChannel),
        nameof(IGuildActor),
        RouteGenerics = [typeof(GuildMediaChannel)]
    ),
    SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")
]
public partial interface IMediaChannelActor :
    IThreadableChannelTrait<IThreadChannelActor.Indexable.WithArchived.BackLink<IMediaChannelActor>>,
    IIntegrationChannelTrait.WithIncoming,
    IActor<ulong, IMediaChannel>;