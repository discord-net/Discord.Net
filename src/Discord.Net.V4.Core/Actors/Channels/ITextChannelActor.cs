using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

[
    Loadable(nameof(Routes.GetChannel), typeof(GuildTextChannel)),
    Modifiable<ModifyTextChannelProperties>(nameof(Routes.ModifyChannel)),
    Creatable<CreateGuildTextChannelProperties>(
        nameof(Routes.CreateGuildChannel),
        nameof(IGuildActor),
        RouteGenerics = [typeof(GuildTextChannel)]
    ),
    SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity"), BackLink(nameof(Messages))
]
public partial interface ITextChannelActor :
    IMessageChannelTrait,
    IThreadableChannelActor,
    IIntegrationChannelTrait.WithIncoming.WithChannelFollower,
    IActor<ulong, ITextChannel>;