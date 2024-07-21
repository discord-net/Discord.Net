using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

[Loadable(nameof(Routes.GetChannel), typeof(GuildTextChannelModel))]
[Modifiable<ModifyTextChannelProperties>(nameof(Routes.ModifyChannel))]
[SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
public partial interface ITextChannelActor :
    IMessageChannelActor,
    IThreadableChannelActor,
    IIntegrationChannelActor,
    IActor<ulong, ITextChannel>;
