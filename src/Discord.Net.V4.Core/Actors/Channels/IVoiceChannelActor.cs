using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

[Loadable(nameof(Routes.GetChannel), typeof(GuildVoiceChannelModel))]
[Modifiable<ModifyVoiceChannelProperties>(nameof(Routes.ModifyChannel))]
[SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
public partial interface IVoiceChannelActor :
    IMessageChannelActor,
    IGuildChannelActor,
    IActor<ulong, IVoiceChannel>;
