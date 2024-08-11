using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

[Loadable(nameof(Routes.GetChannel), typeof(GuildVoiceChannel))]
[Modifiable<ModifyVoiceChannelProperties>(nameof(Routes.ModifyChannel))]
[SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
public partial interface IVoiceChannelActor :
    IMessageChannelTrait,
    IIncomingIntegrationChannelTrait,
    IActor<ulong, IVoiceChannel>;
