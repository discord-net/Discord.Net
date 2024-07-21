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
    IInviteActor Invite(string code) => Invites[code];
    IEnumerableIndexableActor<IInviteActor, string, IInvite> Invites { get; }

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
