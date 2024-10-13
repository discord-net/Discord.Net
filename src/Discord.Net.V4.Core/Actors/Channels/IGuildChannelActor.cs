using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

[
    Loadable(nameof(Routes.GetChannel), typeof(GuildChannelBase)),
    Modifiable<ModifyGuildChannelProperties>(nameof(Routes.ModifyChannel)),
    Deletable(nameof(Routes.DeleteChannel)),
    Creatable<CreateGuildChannelProperties>(
        nameof(Routes.CreateGuildChannel),
        nameof(IGuildActor)
    ),
    LinkHierarchicalRoot,
    SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")
]
public partial interface IGuildChannelActor :
    IGuildActor.CanonicalRelationship,
    IChannelActor,
    IActor<ulong, IGuildChannel>
{
    [BackLink<IGuildActor>]
    private static Task ModifyPositionsAsync(
        IGuildActor guild,
        IEnumerable<ModifyGuildChannelPositionProperties> positions,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        return guild.Client.RestApiClient.ExecuteAsync(
            Routes.ModifyGuildChannelPositions(
                guild.Id,
                positions.Select(x => x.ToApiModel()).ToArray()
            ),
            options ?? guild.Client.DefaultRequestOptions,
            token
        );
    }
}