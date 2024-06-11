using Discord.Models.Json;
using Discord.Utils;

namespace Discord.Rest;

public static partial class Routes
{
    public static readonly ApiRoute<User> GetCurrentUser
        = new(nameof(GetCurrentUser),
            RequestMethod.Get,
            "users/@me");

    public static readonly ApiRoute<UserConnection> GetUserConnections
        = new(nameof(GetUserConnections),
            RequestMethod.Get,
            "users/@me/connections");

    public static ApiRoute<User> GetUser(ulong userId)
        => new(nameof(GetUser),
            RequestMethod.Get,
            $"users/{userId}");

    public static ApiBodyRoute<ModifyCurrentUserParams, User> ModifyCurrentUser(ModifyCurrentUserParams body)
        => new(nameof(ModifyCurrentUser),
            RequestMethod.Patch,
            "users/@me",
            body);

    public static ApiRoute<IEnumerable<PartialGuild>> GetCurrentUserGuilds(ulong? before, ulong? after, int? limit, bool? withCounts)
        => new(
            nameof(GetCurrentUserGuilds),
            RequestMethod.Get,
            $"users/@me/guilds{RouteUtils.GetUrlEncodedQueryParams(("before", before), ("after", after), ("limit", limit), ("with_counts", withCounts))}"
        );

    public static ApiRoute<GuildMember> GetCurrentUserGuildMember(ulong guildId)
        => new(nameof(GetCurrentUserGuildMember),
            RequestMethod.Get,
            $"users/@me/guilds/{guildId}/member",
            (ScopeType.Guild, guildId));

    public static BasicApiRoute LeaveGuild(ulong guildId)
        => new(nameof(LeaveGuild),
            RequestMethod.Delete,
            $"users/@me/guilds/{guildId}",
            (ScopeType.Guild, guildId));

    public static ApiBodyRoute<CreateDMChannelParams, DMChannelModel> CreateDm(CreateDMChannelParams body)
        => new(nameof(CreateDm),
            RequestMethod.Post,
            "users/@me/channels",
            body);

    public static ApiBodyRoute<CreateGroupDMChannelParams, GroupDMChannel> CreateGroupDm(
        CreateGroupDMChannelParams body)
        => new(nameof(CreateGroupDm),
            RequestMethod.Post,
            "users/@me/channels",
            body);

    public static ApiRoute<ApplicationRoleConnection> GetUserApplicationRoleConnection(ulong applicationId)
        => new(nameof(GetUserApplicationRoleConnection),
            RequestMethod.Get,
            $"/users/@me/applications/{applicationId}/role-connection");

    public static ApiBodyRoute<ModifyUserRoleConnectionParams, ApplicationRoleConnection>
        UpdateUserApplicationRoleConnection(ulong applicationId, ModifyUserRoleConnectionParams body)
        => new(nameof(UpdateUserApplicationRoleConnection),
            RequestMethod.Put,
            $"/users/@me/applications/{applicationId}/role-connection",
            body);
}
