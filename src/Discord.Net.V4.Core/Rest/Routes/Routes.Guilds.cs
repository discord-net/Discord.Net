using Discord.Models.Json;
using Discord.Utils;

namespace Discord.Rest;

public partial class Routes
{
    public static IApiInOutRoute<CreateGuildParams, Guild> CreateGuild(CreateGuildParams body) =>
        new ApiInOutRoute<CreateGuildParams, Guild>(nameof(CreateGuild), RequestMethod.Post, "guilds", body);

    public static IApiOutRoute<Guild> GetGuild(ulong id, bool? withCounts = default) =>
        new ApiOutRoute<Guild>(nameof(GetGuild), RequestMethod.Get,
            $"guilds/{id}{RouteUtils.GetUrlEncodedQueryParams(("with_counts", withCounts))}");

    public static IApiOutRoute<GuildPreview> GetGuildPreview([IdHeuristic<IGuild>] ulong guildId) =>
        new ApiOutRoute<GuildPreview>(nameof(GetGuildPreview), RequestMethod.Get, $"guilds/{guildId}/preview",
            (ScopeType.Guild, guildId));

    public static IApiInOutRoute<ModifyGuildParams, Guild> ModifyGuild([IdHeuristic<IGuild>] ulong guildId,
        ModifyGuildParams body) =>
        new ApiInOutRoute<ModifyGuildParams, Guild>(nameof(ModifyGuild), RequestMethod.Patch, $"guilds/{guildId}", body,
            ContentType.JsonBody, (ScopeType.Guild, guildId));

    public static IApiRoute DeleteGuild([IdHeuristic<IGuild>] ulong guildId) =>
        new ApiRoute(nameof(DeleteGuild), RequestMethod.Delete, $"guilds/{guildId}", (ScopeType.Guild, guildId));

    public static IApiOutRoute<IEnumerable<GuildChannelBase>> GetGuildChannels([IdHeuristic<IGuild>] ulong guildId) =>
        new ApiOutRoute<GuildChannelBase[]>(nameof(GetGuildChannels), RequestMethod.Get,
            $"guilds/{guildId}/channels",
            (ScopeType.Guild, guildId));

    public static IApiInOutRoute<CreateGuildChannelParams, GuildChannelBase> CreateGuildChannel(
        [IdHeuristic<IGuild>] ulong guildId,
        CreateGuildChannelParams body) =>
        new ApiInOutRoute<CreateGuildChannelParams, GuildChannelBase>(nameof(CreateGuildChannel),
            RequestMethod.Post,
            $"guilds/{guildId}/channels", body, ContentType.JsonBody, (ScopeType.Guild, guildId));

    public static IApiInRoute<ModifyGuildChannelPositionsParams[]> ModifyGuildChannelPositions(
        [IdHeuristic<IGuild>] ulong guildId,
        ModifyGuildChannelPositionsParams[] body) =>
        new ApiInRoute<ModifyGuildChannelPositionsParams[]>(nameof(ModifyGuildChannelPositions), RequestMethod.Patch,
            $"guild/{guildId}/channels", body, ContentType.JsonBody, (ScopeType.Guild, guildId));

    public static IApiOutRoute<ListActiveGuildThreadsResponse> ListActiveGuildThreads(
        [IdHeuristic<IGuild>] ulong guildId) =>
        new ApiOutRoute<ListActiveGuildThreadsResponse>(nameof(ListActiveGuildThreads), RequestMethod.Get,
            $"guilds/{guildId}/threads/active", (ScopeType.Guild, guildId));

    public static IApiOutRoute<GuildMember> GetGuildMember([IdHeuristic<IGuild>] ulong guildId,
        [IdHeuristic<IMember>] ulong userId) =>
        new ApiOutRoute<GuildMember>(nameof(GetGuildMember), RequestMethod.Get, $"guilds/{guildId}/members/{userId}",
            (ScopeType.Guild, guildId));

    public static IApiOutRoute<GuildMember[]> ListGuildMembers([IdHeuristic<IGuild>] ulong guildId,
        int? limit = default,
        EntityOrId<ulong, IMember>? after = default) =>
        new ApiOutRoute<GuildMember[]>(nameof(ListGuildMembers), RequestMethod.Get,
            $"guilds/{guildId}/members{RouteUtils.GetUrlEncodedQueryParams(("limit", limit), ("after", after?.Id))}",
            (ScopeType.Guild, guildId));

    public static IApiOutRoute<GuildMember[]> SearchGuildMembers([IdHeuristic<IGuild>] ulong guildId, string query,
        int? limit = default) =>
        new ApiOutRoute<GuildMember[]>(nameof(SearchGuildMembers), RequestMethod.Get,
            $"guilds/{guildId}/members/search{RouteUtils.GetUrlEncodedQueryParams(("query", query), ("limit", limit))}",
            (ScopeType.Guild, guildId));

    public static IApiInOutRoute<AddGuildMemberParams, GuildMember> AddGuildMember([IdHeuristic<IGuild>] ulong guildId,
        [IdHeuristic<IUser>] ulong userId,
        AddGuildMemberParams body) =>
        new ApiInOutRoute<AddGuildMemberParams, GuildMember>(nameof(AddGuildMember), RequestMethod.Put,
            $"guilds/{guildId}/members/{userId}", body, ContentType.JsonBody, (ScopeType.Guild, guildId));

    public static IApiInOutRoute<ModifyGuildMemberParams, GuildMember> ModifyGuildMember(
        [IdHeuristic<IGuild>] ulong guildId, [IdHeuristic<IMember>] ulong userId,
        ModifyGuildMemberParams body) =>
        new ApiInOutRoute<ModifyGuildMemberParams, GuildMember>(nameof(ModifyGuildMember), RequestMethod.Patch,
            $"guilds/{guildId}/members/{userId}", body, ContentType.JsonBody, (ScopeType.Guild, guildId));

    public static IApiInOutRoute<ModifyCurrentMemberParams, GuildMember> ModifyCurrentMember(
        [IdHeuristic<IGuild>] ulong guildId,
        ModifyCurrentMemberParams body) =>
        new ApiInOutRoute<ModifyCurrentMemberParams, GuildMember>(nameof(ModifyCurrentMember), RequestMethod.Patch,
            $"guilds/{guildId}/members/@me", body, ContentType.JsonBody, (ScopeType.Guild, guildId));

    public static IApiRoute AddGuildMemberRole([IdHeuristic<IGuild>] ulong guildId,
        [IdHeuristic<IMember>] ulong userId, [IdHeuristic<IRole>] ulong roleId) =>
        new ApiRoute(nameof(AddGuildMemberRole), RequestMethod.Put,
            $"guilds/{guildId}/members/{userId}/roles/{roleId}", (ScopeType.Guild, guildId));

    public static IApiRoute RemoveGuildMemberRole([IdHeuristic<IGuild>] ulong guildId,
        [IdHeuristic<IMember>] ulong userId, [IdHeuristic<IRole>] ulong roleId) =>
        new ApiRoute(nameof(RemoveGuildMemberRole), RequestMethod.Delete,
            $"guilds/{guildId}/members/{userId}/roles/{roleId}", (ScopeType.Guild, guildId));

    public static IApiRoute RemoveGuildMember([IdHeuristic<IGuild>] ulong guildId,
        [IdHeuristic<IMember>] ulong userId) =>
        new ApiRoute(nameof(RemoveGuildMember), RequestMethod.Delete, $"guilds/{guildId}/members/{userId}",
            (ScopeType.Guild, guildId));

    public static IApiOutRoute<Ban[]> GetGuildBans([IdHeuristic<IGuild>] ulong guildId, int? limit = default,
        EntityOrId<ulong, IUser>? before = default,
        EntityOrId<ulong, IUser>? after = default) =>
        new ApiOutRoute<Ban[]>(nameof(GetGuildBans), RequestMethod.Get,
            $"guilds/{guildId}/bans{RouteUtils.GetUrlEncodedQueryParams(("limit", limit), ("before", before?.Id), ("after", after?.Id))}",
            (ScopeType.Guild, guildId));

    public static IApiOutRoute<Ban>
        GetGuildBan([IdHeuristic<IGuild>] ulong guildId, [IdHeuristic<IBan>] ulong userId) =>
        new ApiOutRoute<Ban>(nameof(GetGuildBan), RequestMethod.Get, $"guilds/{guildId}/bans/{userId}",
            (ScopeType.Guild, guildId));

    public static IApiInRoute<CreateGuildBanParams> CreateGuildBan([IdHeuristic<IGuild>] ulong guildId,
        [IdHeuristic<IUser>] ulong userId,
        CreateGuildBanParams body) =>
        new ApiInRoute<CreateGuildBanParams>(nameof(CreateGuildBan), RequestMethod.Put,
            $"guilds/{guildId}/bans/{userId}", body, ContentType.JsonBody, (ScopeType.Guild, guildId));

    public static IApiRoute RemoveGuildBan([IdHeuristic<IGuild>] ulong guildId, [IdHeuristic<IBan>] ulong banId) =>
        new ApiRoute(nameof(RemoveGuildBan), RequestMethod.Delete, $"guilds/{guildId}/bans/{banId}",
            (ScopeType.Guild, guildId));

    public static IApiInOutRoute<BulkBanUsersParams, BulkBanResponse> BulkGuildBan([IdHeuristic<IGuild>] ulong guildId,
        BulkBanUsersParams args) =>
        new ApiInOutRoute<BulkBanUsersParams, BulkBanResponse>(nameof(BulkGuildBan), RequestMethod.Post,
            $"guilds/{guildId}/bulk-ban", args, ContentType.JsonBody, (ScopeType.Guild, guildId));

    public static IApiOutRoute<Role[]> GetGuildRoles([IdHeuristic<IGuild>] ulong guildId) =>
        new ApiOutRoute<Role[]>(nameof(GetGuildRoles), RequestMethod.Get, $"guilds/{guildId}/roles",
            (ScopeType.Guild, guildId));

    public static IApiOutRoute<Role> GetGuildRole(
        [IdHeuristic<IGuild>] ulong guildId,
        [IdHeuristic<IRole>] ulong roleId
    ) => new ApiOutRoute<Role>(
        nameof(GetGuildRoles),
        RequestMethod.Get,
        $"guilds/{guildId}/roles",
        (ScopeType.Guild, guildId)
    );

    public static IApiInOutRoute<CreateGuildRoleParams, Role>
        CreateGuildRole([IdHeuristic<IGuild>] ulong guildId, CreateGuildRoleParams body) =>
        new ApiInOutRoute<CreateGuildRoleParams, Role>(nameof(CreateGuildRole), RequestMethod.Post,
            $"guilds/{guildId}/roles", body, ContentType.JsonBody, (ScopeType.Guild, guildId));

    public static IApiInOutRoute<ModifyGuildRolePositionParams[], Role[]> ModifyGuildRolePositions(
        [IdHeuristic<IGuild>] ulong guildId,
        ModifyGuildRolePositionParams[] body) =>
        new ApiInOutRoute<ModifyGuildRolePositionParams[], Role[]>(nameof(ModifyGuildRolePositions),
            RequestMethod.Patch, $"guilds/{guildId}/roles", body, ContentType.JsonBody, (ScopeType.Guild, guildId));

    public static IApiInOutRoute<ModifyGuildRoleParams, Role> ModifyGuildRole([IdHeuristic<IGuild>] ulong guildId,
        [IdHeuristic<IRole>] ulong roleId,
        ModifyGuildRoleParams body) =>
        new ApiInOutRoute<ModifyGuildRoleParams, Role>(nameof(ModifyGuildRole), RequestMethod.Patch,
            $"guilds/{guildId}/roles/{roleId}", body, ContentType.JsonBody, (ScopeType.Guild, guildId));

    public static IApiInOutRoute<ModifyGuildMfaLevelParams, MfaLevelResponse> ModifyGuildMfaLevel(
        [IdHeuristic<IGuild>] ulong guildId,
        ModifyGuildMfaLevelParams body) =>
        new ApiInOutRoute<ModifyGuildMfaLevelParams, MfaLevelResponse>(nameof(ModifyGuildMfaLevel), RequestMethod.Patch,
            $"guilds/{guildId}/mfa", body, ContentType.JsonBody, (ScopeType.Guild, guildId));

    public static IApiRoute DeleteGuildRole([IdHeuristic<IGuild>] ulong guildId, [IdHeuristic<IRole>] ulong roleId) =>
        new ApiRoute(nameof(DeleteGuildRole), RequestMethod.Delete, $"guilds/{guildId}/roles/{roleId}",
            (ScopeType.Guild, guildId));

    public static IApiOutRoute<GuildPruneCount> GetGuildPruneCount([IdHeuristic<IGuild>] ulong guildId,
        int? days = default,
        ulong[]? includeRoles = default) =>
        new ApiOutRoute<GuildPruneCount>(nameof(GetGuildPruneCount), RequestMethod.Get,
            $"guilds/{guildId}/prune{RouteUtils.GetUrlEncodedQueryParams(("days", days), ("include_roles", includeRoles))}",
            (ScopeType.Guild, guildId));

    public static IApiInOutRoute<BeginGuildPruneParams, GuildPruneCount> BeginGuildPrune(
        [IdHeuristic<IGuild>] ulong guildId,
        BeginGuildPruneParams body) =>
        new ApiInOutRoute<BeginGuildPruneParams, GuildPruneCount>(nameof(BeginGuildPrune), RequestMethod.Post,
            $"guilds/{guildId}/prune", body, ContentType.JsonBody, (ScopeType.Guild, guildId));

    public static IApiOutRoute<Models.Json.VoiceRegion[]> GetGuildVoiceRegions([IdHeuristic<IGuild>] ulong guildId) =>
        new ApiOutRoute<Models.Json.VoiceRegion[]>(nameof(GetGuildVoiceRegions), RequestMethod.Get,
            $"guilds/{guildId}/regions", (ScopeType.Guild, guildId));

    public static IApiOutRoute<InviteMetadata[]> GetGuildInvites([IdHeuristic<IGuild>] ulong guildId) =>
        new ApiOutRoute<InviteMetadata[]>(nameof(GetGuildInvites), RequestMethod.Get, $"guilds/{guildId}/invites",
            (ScopeType.Guild, guildId));

    public static IApiOutRoute<Models.Json.Integration[]> GetGuildIntegrations([IdHeuristic<IGuild>] ulong guildId) =>
        new ApiOutRoute<Models.Json.Integration[]>(nameof(GetGuildIntegrations), RequestMethod.Get,
            $"guilds/{guildId}/integrations", (ScopeType.Guild, guildId));

    public static IApiRoute DeleteGuildIntegration([IdHeuristic<IGuild>] ulong guildId,
        [IdHeuristic<IIntegration>] ulong integrationId) =>
        new ApiRoute(nameof(DeleteGuildIntegration), RequestMethod.Delete,
            $"guilds/{guildId}/integrations/{integrationId}", (ScopeType.Guild, guildId));

    public static IApiOutRoute<GuildWidgetSettings> GetGuildWidgetSettings([IdHeuristic<IGuild>] ulong guildId) =>
        new ApiOutRoute<GuildWidgetSettings>(nameof(GetGuildWidgetSettings), RequestMethod.Get,
            $"guilds/{guildId}/widget", (ScopeType.Guild, guildId));

    public static IApiInOutRoute<ModifyGuildWidgetParams, GuildWidgetSettings> ModifyGuildWidgetSettings(
        [IdHeuristic<IGuild>] ulong guildId,
        ModifyGuildWidgetParams body) =>
        new ApiInOutRoute<ModifyGuildWidgetParams, GuildWidgetSettings>(nameof(ModifyGuildWidgetSettings),
            RequestMethod.Patch, $"guilds/{guildId}/widget", body, ContentType.JsonBody, (ScopeType.Guild, guildId));

    public static IApiOutRoute<GuildWidget> GetGuildWidget([IdHeuristic<IGuild>] ulong guildId) =>
        new ApiOutRoute<GuildWidget>(nameof(GetGuildWidget), RequestMethod.Get, $"guilds/{guildId}/widget.json",
            (ScopeType.Guild, guildId));

    public static IApiOutRoute<Invite> GetGuildVanityUrl([IdHeuristic<IGuild>] ulong guildId) =>
        new ApiOutRoute<Invite>(nameof(GetGuildVanityUrl), RequestMethod.Get, $"guilds/{guildId}/vanity-url",
            (ScopeType.Guild, guildId));

    public static IApiOutRoute<WelcomeScreen> GetGuildWelcomeScreen([IdHeuristic<IGuild>] ulong guildId) =>
        new ApiOutRoute<WelcomeScreen>(nameof(GetGuildWelcomeScreen), RequestMethod.Get,
            $"guilds/{guildId}/welcome-screen", (ScopeType.Guild, guildId));

    public static IApiInOutRoute<ModifyGuildWelcomeScreenParams, WelcomeScreen> ModifyGuildWelcomeScreen(
        [IdHeuristic<IGuild>] ulong guildId,
        ModifyGuildWelcomeScreenParams body) =>
        new ApiInOutRoute<ModifyGuildWelcomeScreenParams, WelcomeScreen>(nameof(ModifyGuildWelcomeScreen),
            RequestMethod.Patch, $"guilds/{guildId}/welcome-screen", body, ContentType.JsonBody,
            (ScopeType.Guild, guildId));

    // TODO: add
    //public static string GuildOnboarding([IdHeuristic<IGuild>] ulong guildId)
    //        => $"guilds/{guildId}/onboarding";
}