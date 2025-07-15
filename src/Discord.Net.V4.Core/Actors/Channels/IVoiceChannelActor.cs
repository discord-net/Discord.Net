using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

[
    Loadable<Routes.GetChannel>,
    Modifiable<Routes.UpdateChannel, ModifyVoiceChannelProperties>,
    Creatable<Routes.CreateGuildChannel, CreateGuildVoiceChannelProperties>
    (
        WhenBackLinkingFrom = [typeof(IGuildActor)]
    )
]
public partial interface IVoiceChannelActor :
    IMessageChannelTrait,
    IInvitableTrait<IGuildChannelInviteActor, IGuildChannelInvite>,
    IIntegrationChannelTrait.WithIncoming,
    IActor<ulong, IVoiceChannel>;