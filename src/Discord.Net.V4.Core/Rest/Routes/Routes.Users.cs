using Discord.API;
using Discord.Utils;

namespace Discord.Rest;

public static partial class Routes
{
    public static APIRoute<User> GetUser(ulong userId)
        => new(nameof(GetUser), RequestMethod.Get, $"users/{userId}");

    public static readonly APIRoute<User> CurrentUser
        = new(nameof(CurrentUser), RequestMethod.Get, "users/@me");

    public static APIBodyRoute<ModifySelfUserProperties, User> ModifyCurrentUser(ModifySelfUserProperties properties)
        => new(nameof(ModifyCurrentUser), RequestMethod.Patch, "users/@me", properties);

    public static APIRoute<PartialGuild> GetCurrentUserGuilds(ulong? before, ulong? after, int? limit, bool? withCounts)
        => new(
            nameof(GetCurrentUserGuilds),
            RequestMethod.Get,
            $"users/@me/guilds{RouteUtils.GetUrlEncodedQueryParams(("before", before), ("after", after), ("limit", limit), ("with_counts", withCounts))}"
        );

    public static APIRoute<GuildMember> GetCurrentUserGuildMember(ulong guildId)
        => new(nameof(GetCurrentUserGuildMember), RequestMethod.Get, $"users/@me/guilds/{guildId}/member", (ScopeType.Guild, guildId));

    public static APIRoute LeaveGuild(ulong guildId)
        => new(nameof(LeaveGuild), RequestMethod.Delete, $"users/@me/guilds/{guildId}", (ScopeType.Guild, guildId));

    public static APIBodyRoute<CreateDMProperties, Channel> CreateDM(CreateDMProperties body)
        => new(nameof(CreateDM), RequestMethod.Post, "users/@me/channels", body);

    public static APIBodyRoute<CreateGroupDMProperties, Channel> CreateGroupDM(CreateGroupDMProperties body)
        => new(nameof(CreateGroupDM), RequestMethod.Post, "users/@me/channels", body);

    public static readonly APIRoute<Connection> GetUserConnections
        = new(nameof(GetUserConnections), RequestMethod.Get, "users/@me/connections");

    public static APIRoute<ApplicationRoleConnection> GetUserApplicationRoleConnection(ulong applicationId)
        => new(nameof(GetUserApplicationRoleConnection), RequestMethod.Get, $"/users/@me/applications/{applicationId}/role-connection");

    public static APIBodyRoute<ModifyUserApplicationRoleConnection, ApplicationRoleConnection> UpdateUserApplicationRoleConnection(ulong applicationId, ModifyUserApplicationRoleConnection body)
        => new(nameof(UpdateUserApplicationRoleConnection), RequestMethod.Put, $"/users/@me/applications/{applicationId}/role-connection", body);
}
