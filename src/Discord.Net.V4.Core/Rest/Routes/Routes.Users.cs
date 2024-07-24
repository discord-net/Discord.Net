using Discord.Models.Json;
using Discord.Utils;

namespace Discord.Rest;

public static partial class Routes
{
    public static readonly IApiOutRoute<User> GetCurrentUser
        = new ApiOutRoute<User>(nameof(GetCurrentUser),
            RequestMethod.Get,
            "users/@me");

    public static readonly IApiOutRoute<Models.Json.UserConnection[]> GetUserConnections
        = new ApiOutRoute<Models.Json.UserConnection[]>(nameof(GetUserConnections),
            RequestMethod.Get,
            "users/@me/connections");

    public static IApiOutRoute<User> GetUser([IdHeuristic<IUser>] ulong userId)
        => new ApiOutRoute<User>(nameof(GetUser),
            RequestMethod.Get,
            $"users/{userId}");

    public static IApiInOutRoute<ModifyCurrentUserParams, User> ModifyCurrentUser(ModifyCurrentUserParams body) =>
        new ApiInOutRoute<ModifyCurrentUserParams, User>(nameof(ModifyCurrentUser), RequestMethod.Patch, "users/@me",
            body);

    public static IApiOutRoute<IEnumerable<PartialGuild>> GetCurrentUserGuilds(
        ulong? before = null,
        ulong? after = null, int? limit = null,
        bool? withCounts = null
    ) => new ApiOutRoute<IEnumerable<PartialGuild>>(
        nameof(GetCurrentUserGuilds),
        RequestMethod.Get,
        $"users/@me/guilds{RouteUtils.GetUrlEncodedQueryParams(("before", before), ("after", after), ("limit", limit), ("with_counts", withCounts))}"
    );

    public static IApiOutRoute<GuildMember> GetCurrentUserGuildMember([IdHeuristic<IGuild>] ulong guildId)
        => new ApiOutRoute<GuildMember>(nameof(GetCurrentUserGuildMember),
            RequestMethod.Get,
            $"users/@me/guilds/{guildId}/member",
            (ScopeType.Guild, guildId));

    public static IApiRoute LeaveGuild([IdHeuristic<IGuild>] ulong guildId) =>
        new ApiRoute(nameof(LeaveGuild), RequestMethod.Delete, $"users/@me/guilds/{guildId}",
            (ScopeType.Guild, guildId));

    public static IApiInOutRoute<CreateDMChannelParams, DMChannelModel> CreateDm(CreateDMChannelParams body) =>
        new ApiInOutRoute<CreateDMChannelParams, DMChannelModel>(nameof(CreateDm), RequestMethod.Post,
            "users/@me/channels", body);

    public static IApiInOutRoute<CreateGroupDMChannelParams, GroupDMChannelModel> CreateGroupDm(
        CreateGroupDMChannelParams body) =>
        new ApiInOutRoute<CreateGroupDMChannelParams, GroupDMChannelModel>(nameof(CreateGroupDm), RequestMethod.Post,
            "users/@me/channels", body);

    public static IApiOutRoute<ApplicationRoleConnection> GetUserApplicationRoleConnection(ulong applicationId)
        => new ApiOutRoute<ApplicationRoleConnection>(nameof(GetUserApplicationRoleConnection),
            RequestMethod.Get,
            $"/users/@me/applications/{applicationId}/role-connection");

    public static IApiInOutRoute<ModifyUserRoleConnectionParams, ApplicationRoleConnection>
        UpdateUserApplicationRoleConnection(ulong applicationId, ModifyUserRoleConnectionParams body) =>
        new ApiInOutRoute<ModifyUserRoleConnectionParams, ApplicationRoleConnection>(
            nameof(UpdateUserApplicationRoleConnection), RequestMethod.Put,
            $"/users/@me/applications/{applicationId}/role-connection", body);
}
