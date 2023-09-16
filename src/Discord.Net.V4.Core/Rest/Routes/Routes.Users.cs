using Discord.API;
using Discord.Utils;

namespace Discord.Rest;

public static partial class Routes
{
    public static readonly APIRoute<User> GetCurrentUser
        = new(nameof(GetCurrentUser), RequestMethod.Get, "users/@me");

    public static APIRoute<User> GetUser(ulong userId)
        => new(nameof(GetUser), RequestMethod.Get, $"users/{userId}");

    public static APIBodyRoute<ModifyCurrentUserParams, User> ModifyCurrentUser(ModifyCurrentUserParams body)
        => new(nameof(ModifyCurrentUser), RequestMethod.Patch, "users/@me", body);

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

    public static APIBodyRoute<CreateDMChannelParams, Channel> CreateDM(CreateDMChannelParams body)
        => new(nameof(CreateDM), RequestMethod.Post, "users/@me/channels", body);

    public static APIBodyRoute<CreateGroupDMChannelParams, Channel> CreateGroupDM(CreateGroupDMChannelParams body)
        => new(nameof(CreateGroupDM), RequestMethod.Post, "users/@me/channels", body);

    public static readonly APIRoute<UserConnection> GetUserConnections
        = new(nameof(GetUserConnections), RequestMethod.Get, "users/@me/connections");

    public static APIRoute<ApplicationRoleConnection> GetUserApplicationRoleConnection(ulong applicationId)
        => new(nameof(GetUserApplicationRoleConnection), RequestMethod.Get, $"/users/@me/applications/{applicationId}/role-connection");

    public static APIBodyRoute<ModifyUserRoleConnectionParams, ApplicationRoleConnection> UpdateUserApplicationRoleConnection(ulong applicationId, ModifyUserRoleConnectionParams body)
        => new(nameof(UpdateUserApplicationRoleConnection), RequestMethod.Put, $"/users/@me/applications/{applicationId}/role-connection", body);
}
