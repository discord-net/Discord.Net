using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

[Loadable(nameof(Routes.GetChannel), typeof(GuildMediaChannelModel))]
[Modifiable<ModifyMediaChannelProperties>(nameof(Routes.ModifyChannel))]
[SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
public partial interface IMediaChannelActor :
    IThreadableChannelActor,
    IIntegrationChannelActor,
    IActor<ulong, IMediaChannel>;
