using Discord.Invites;
using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public interface ILoadableGuildChannelActor :
    IGuildChannelActor,
    ILoadableEntity<ulong, IGuildChannel>;

[Modifiable<ModifyGuildChannelProperties>(nameof(Routes.ModifyChannel))]
[Deletable(nameof(Routes.DeleteChannel))]
public partial interface IGuildChannelActor :
    IChannelActor,
    IGuildRelationship,
    IActor<ulong, IGuildChannel>
{
    IEnumerableIndexableActor<ILoadableInviteActor<IInvite>, string, IInvite> Invites { get; }

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

        return Client.CreateEntity(model);
    }
}
