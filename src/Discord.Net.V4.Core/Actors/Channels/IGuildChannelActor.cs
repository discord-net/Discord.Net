using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

[Loadable(nameof(Routes.GetChannel), typeof(GuildChannelBase))]
[Modifiable<ModifyGuildChannelProperties>(nameof(Routes.ModifyChannel))]
[Deletable(nameof(Routes.DeleteChannel))]
[Creatable<CreateGuildChannelProperties>(nameof(Routes.CreateGuildChannel))]
[Invitable<CreateChannelInviteProperties, IGuildChannelInvite>(nameof(Routes.CreateChannelInvite))]
[SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
public partial interface IGuildChannelActor :
    IGuildRelationship,
    IInvitableChannelTrait,
    IActor<ulong, IGuildChannel>
{
    [return: TypeHeuristic(nameof(Invites))]
    IGuildChannelInviteActor Invite(string code) => Invites[code];
    EnumerableIndexableGuildChannelInviteLink Invites { get; }

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
