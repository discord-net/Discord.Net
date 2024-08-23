using Discord.Models.Json;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

[
    Loadable(nameof(Routes.GetChannel), typeof(GuildCategoryChannel)),
    Creatable<CreateGuildCategoryChannelProperties>(
        nameof(Routes.CreateGuildChannel),
        nameof(IGuildActor.CategoryChannels),
        RouteGenerics = [typeof(GuildCategoryChannel)]
    ),
    SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")
]
public partial interface ICategoryChannelActor :
    IGuildChannelActor,
    IActor<ulong, ICategoryChannel>;