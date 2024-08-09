using Discord.Invites;
using Discord.Models;
using Discord.Rest;

namespace Discord;

public interface IInvitableChannelActor :
    IChannelActor,
    IActor<ulong, IInvitableChannel>,
    IEntityProvider<IInvite, IInviteModel>
{
    [return: TypeHeuristic(nameof(Invites))]
    IGuildChannelInviteActor Invite(string code) => Invites[code];
    IEnumerableIndexableActor<IGuildChannelInviteActor, string, IInvite> Invites { get; }

    async Task<IInvite> CreateInviteAsync(
        CreateChannelInviteProperties args,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        var model = await Client.RestApiClient.ExecuteRequiredAsync(
            Routes.CreateChannelInvite(Id, args.ToApiModel()),
            options ?? Client.DefaultRequestOptions,
            token
        );

        return CreateEntity(model);
    }
}
