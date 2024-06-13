using Discord.Models.Json;
using Discord.Rest;
using System.Collections.Immutable;

namespace Discord;

public interface ILoadableSelfUserActor<TSelfUser> :
    ILoadableEntity<ulong, TSelfUser>,
    ISelfUserActor<TSelfUser>
    where TSelfUser : class, ISelfUser;

public interface ISelfUserActor<out TSelfUser> :
    IModifiable<ulong, ISelfUserActor<TSelfUser>, ModifySelfUserProperties, ModifyCurrentUserParams>,
    IActor<ulong, TSelfUser>
    where TSelfUser : ISelfUser
{
    async Task<IEnumerable<IPartialGuild>?> GetGuildsAsync(
        EntityOrId<ulong, IPartialGuild>? before,
        EntityOrId<ulong, IPartialGuild>? after,
        int limit = 200,
        bool withCounts = false,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        var result = await Client.RestApiClient.ExecuteAsync(
            Routes.GetCurrentUserGuilds(before?.Id, after?.Id, limit, withCounts),
            options ?? Client.DefaultRequestOptions,
            token
        );

        return result?.Select(Client.CreateEntity);
    }

    async Task<IGuildMember?> GetCurrentGuildMemberAsync(
        EntityOrId<ulong, IPartialGuild> guild,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        var model = await Client.RestApiClient.ExecuteAsync(
            Routes.GetCurrentUserGuildMember(guild.Id),
            options ?? Client.DefaultRequestOptions,
            token
        );

        return Client.CreateNullableEntity(model);
    }

    Task LeaveGuildAsync(
        EntityOrId<ulong, IPartialGuild> guild,
        RequestOptions? options = null,
        CancellationToken token = default
    ) => Client.RestApiClient.ExecuteAsync(
        Routes.LeaveGuild(guild.Id),
        options ?? Client.DefaultRequestOptions,
        token
    );

    async Task<IDMChannel> CreateDMAsync(
        EntityOrId<ulong, IUser> recipient,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        var model = await Client.RestApiClient.ExecuteRequiredAsync(
            Routes.CreateDm(new CreateDMChannelParams() {RecipientId = recipient.Id}),
            options ?? Client.DefaultRequestOptions,
            token
        );

        return Client.CreateEntity(model);
    }

    async Task<IReadOnlyCollection<UserConnection>> GetConnectionsAsync(
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        var models = await Client.RestApiClient.ExecuteRequiredAsync(
            Routes.GetUserConnections,
            options ?? Client.DefaultRequestOptions,
            token
        );

        return models.Select(x => UserConnection.Construct(Client, x)).ToImmutableArray();
    }

    // TODO:
    // - https://discord.com/developers/docs/resources/user#get-current-user-application-role-connection
    // - https://discord.com/developers/docs/resources/user#update-current-user-application-role-connection

    static ApiBodyRoute<ModifyCurrentUserParams> IModifiable<ulong, ISelfUserActor<TSelfUser>, ModifySelfUserProperties, ModifyCurrentUserParams>.ModifyRoute(IPathable path, ulong id,
        ModifyCurrentUserParams args)
        => Routes.ModifyCurrentUser(args);
}
