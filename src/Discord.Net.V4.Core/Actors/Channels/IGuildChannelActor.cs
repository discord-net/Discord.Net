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
        nameof(IGuildActor.Channels)
    ),
    BackLink(nameof(Invites)),
    SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")
]
public partial interface IGuildChannelActor :
    IGuildRelationship,
    IInvitableTrait<IGuildChannelInviteActor, IGuildChannelInvite>,
    IActor<ulong, IGuildChannel>
{
    [OnVertex]
    private static async Task<IGuildChannelInvite> CreateAsync(
        GuildChannelInviteLink.Enumerable.Indexable.BackLink<IGuildChannelActor> invites,
        Action<CreateChannelInviteProperties>? func = null,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        var args = new CreateChannelInviteProperties();

        func?.Invoke(args);

        var model = await invites.Client.RestApiClient.ExecuteRequiredAsync(
            Routes.CreateChannelInvite(invites.Source.Id, args.ToApiModel()),
            options ?? invites.Client.DefaultRequestOptions,
            token
        );

        return invites.CreateEntity(model);
    }

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