using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

[
    Loadable(nameof(Routes.GetChannel), typeof(GuildVoiceChannel)),
    Modifiable<ModifyVoiceChannelProperties>(nameof(Routes.ModifyChannel)),
    Creatable<CreateGuildVoiceChannelProperties>(
        nameof(Routes.CreateGuildChannel),
        nameof(IGuildActor),
        RouteGenerics = [typeof(GuildVoiceChannel)]
    ),
    SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity"), 
    BackLink(nameof(Messages))
]
public partial interface IVoiceChannelActor :
    IMessageChannelTrait,
    IInvitableTrait<IGuildChannelInviteActor, IGuildChannelInvite>,
    IIntegrationChannelTrait.WithIncoming,
    IGuildChannelActor,
    IActor<ulong, IVoiceChannel>;