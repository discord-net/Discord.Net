using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

[Loadable(nameof(Routes.GetChannel), typeof(GuildTextChannel))]
[Modifiable<ModifyTextChannelProperties>(nameof(Routes.ModifyChannel))]
[SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
public partial interface ITextChannelActor :
    IMessageChannelTrait,
    IThreadableChannelActor,
    IChannelFollowerIntegrationChannelTrait,
    IIncomingIntegrationChannelTrait,
    IActor<ulong, ITextChannel>;
