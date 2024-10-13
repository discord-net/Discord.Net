using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

[
    Loadable(nameof(Routes.GetGuildBan)), 
    Deletable(nameof(Routes.RemoveGuildBan))
]
public partial interface IBanActor :
    IGuildActor.CanonicalRelationship,
    IUserActor.Relationship,
    IActor<ulong, IBan>
{
    [BackLink<IGuildActor>]
    private static Task CreateAsync(
        IGuildActor guild,
        EntityOrId<ulong, IUserActor> user,
        int? purgeMessageSeconds = null,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        return guild.Client.RestApiClient.ExecuteAsync(
            Routes.CreateGuildBan(
                guild.Id,
                user.Id,
                new CreateGuildBanParams
                {
                    DeleteMessageSeconds = Optional.FromNullable(purgeMessageSeconds)
                }
            ),
            options ?? guild.Client.DefaultRequestOptions,
            token
        );
    }

    [BackLink<IGuildActor>]
    private static async Task<BulkBanResult> BulkCreateAsync(
        IGuildActor guild,
        IEnumerable<EntityOrId<ulong, IUser>> users,
        int? purgeMessageSeconds = null,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        var userIds = users.Select(x => x.Id).ToArray();

        if (userIds.Length > DiscordConfig.MaxBulkBansPerBatch)
            throw new ArgumentOutOfRangeException(
                nameof(users),
                userIds.Length,
                $"A max of {DiscordConfig.MaxBulkBansPerBatch} bans can be created at a time"
            );

        return BulkBanResult.Construct(
            guild.Client,
            await guild.Client.RestApiClient.ExecuteRequiredAsync(
                Routes.BulkGuildBan(
                    guild.Id,
                    new BulkBanUsersParams()
                    {
                        UserIds = userIds,
                        DeleteMessageSeconds = Optional.FromNullable(purgeMessageSeconds)
                    }
                ),
                options ?? guild.Client.DefaultRequestOptions,
                token
            )
        );
    }
}