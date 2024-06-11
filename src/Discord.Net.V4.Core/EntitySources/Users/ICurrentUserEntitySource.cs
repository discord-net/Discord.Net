using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord.Users;

public interface ILoadableCurrentUserEntitySource<TSelfUser> :
    ILoadableEntity<ulong, TSelfUser>,
    ICurrentUserEntitySource<TSelfUser>
    where TSelfUser : class, ISelfUser;

public interface ICurrentUserEntitySource<out TSelfUser> :
    IEntitySource<ulong, TSelfUser>,
    IEntityProvider<IPartialGuild, IPartialGuildModel>
    where TSelfUser : ISelfUser
{
    async Task<IEnumerable<IPartialGuild>?> GetGuildsAsync(
        EntityOrId<ulong, IPartialGuild>? before,
        EntityOrId<ulong, IPartialGuild>? after,
        int limit = 200,
        bool withCounts = false,
        RequestOptions? options = null,
        CancellationToken token = default
    )
    {
        var result = await Client.RestApiClient.ExecuteAsync(
            Routes.GetCurrentUserGuilds(before?.Id, after?.Id, limit, withCounts),
            options ?? Client.DefaultRequestOptions,
            token
        );

        return result?.Select(Create);
    }
}
