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
        typeof(IGuildActor),
        RouteGenerics = [typeof(IGuildTextChannelModel)]
    ),
    SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity"), BackLink(nameof(Messages))
]
public partial interface ITextChannelActor :
    IMessageChannelTrait,
    IThreadableChannelActor,
    IChannelFollowerIntegrationChannelTrait,
    IIncomingIntegrationChannelTrait,
    IActor<ulong, ITextChannel>;