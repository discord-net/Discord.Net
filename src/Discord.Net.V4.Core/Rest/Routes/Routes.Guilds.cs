using Discord.Models.Json;
using Discord.Utils;

namespace Discord.Rest;

public partial class Routes
{
    public static IApiInOutRoute<CreateGuildParams, Guild> CreateGuild(CreateGuildParams body) =>
        new ApiInOutRoute<CreateGuildParams, Guild>(nameof(CreateGuild), RequestMethod.Post, "guilds", body);

    public static IApiOutRoute<Guild> GetGuild(ulong guildId, bool? withCounts = default) =>
        new ApiOutRoute<Guild>(nameof(GetGuild), RequestMethod.Get,
            $"guilds/{guildId}{RouteUtils.GetUrlEncodedQueryParams(("with_counts", withCounts))}");

    public static IApiOutRoute<GuildPreview> GetGuildPreview(ulong guildId) =>
        new ApiOutRoute<GuildPreview>(nameof(GetGuildPreview), RequestMethod.Get, $"guilds/{guildId}/preview",
            (ScopeType.Guild, guildId));

    public static IApiInOutRoute<ModifyGuildParams, Guild> ModifyGuild(ulong guildId, ModifyGuildParams body) =>
        new ApiInOutRoute<ModifyGuildParams, Guild>(nameof(ModifyGuild), RequestMethod.Patch, $"guilds/{guildId}", body,
            ContentType.JsonBody, (ScopeType.Guild, guildId));

    public static IApiRoute DeleteGuild(ulong guildId) =>
        new ApiRoute(nameof(DeleteGuild), RequestMethod.Delete, $"guilds/{guildId}", (ScopeType.Guild, guildId));

    public static IApiOutRoute<Channel[]> GetGuildChannels(ulong guildId) =>
        new ApiOutRoute<Channel[]>(nameof(GetGuildChannels), RequestMethod.Get, $"guilds/{guildId}/channels",
            (ScopeType.Guild, guildId));

    public static IApiInOutRoute<CreateGuildChannelParams, GuildChannelBase> CreateGuildChannel(ulong guildId,
        CreateGuildChannelParams body) =>
        new ApiInOutRoute<CreateGuildChannelParams, GuildChannelBase>(nameof(CreateGuildChannel), RequestMethod.Post,
            $"guilds/{guildId}/channels", body, ContentType.JsonBody, (ScopeType.Guild, guildId));

    public static IApiInRoute<ModifyGuildChannelPositionsParams[]> ModifyGuildChannelPositions(ulong guildId,
        ModifyGuildChannelPositionsParams[] body) =>
        new ApiInRoute<ModifyGuildChannelPositionsParams[]>(nameof(ModifyGuildChannelPositions), RequestMethod.Patch,
            $"guild/{guildId}/channels", body, ContentType.JsonBody, (ScopeType.Guild, guildId));

    public static IApiOutRoute<ListActiveGuildThreadsResponse> ListActiveGuildThreads(ulong guildId) =>
        new ApiOutRoute<ListActiveGuildThreadsResponse>(nameof(ListActiveGuildThreads), RequestMethod.Get,
            $"guilds/{guildId}/threads/active", (ScopeType.Guild, guildId));

    public static IApiOutRoute<GuildMember> GetGuildMember(ulong guildId, ulong userId) =>
        new ApiOutRoute<GuildMember>(nameof(GetGuildMember), RequestMethod.Get, $"guilds/{guildId}/members/{userId}",
            (ScopeType.Guild, guildId));

    public static IApiOutRoute<GuildMember[]> ListGuildMembers(ulong guildId, int? limit = default,
        ulong? afterId = default) =>
        new ApiOutRoute<GuildMember[]>(nameof(ListGuildMembers), RequestMethod.Get,
            $"guilds/{guildId}/members{RouteUtils.GetUrlEncodedQueryParams(("limit", limit), ("after", afterId))}",
            (ScopeType.Guild, guildId));

    public static IApiOutRoute<GuildMember[]> SearchGuildMembers(ulong guildId, string query, int? limit = default) =>
        new ApiOutRoute<GuildMember[]>(nameof(SearchGuildMembers), RequestMethod.Get,
            $"guilds/{guildId}/members/search{RouteUtils.GetUrlEncodedQueryParams(("query", query), ("limit", limit))}",
            (ScopeType.Guild, guildId));

    public static IApiInOutRoute<AddGuildMemberParams, GuildMember> AddGuildMember(ulong guildId, ulong userId,
        AddGuildMemberParams body) =>
        new ApiInOutRoute<AddGuildMemberParams, GuildMember>(nameof(AddGuildMember), RequestMethod.Put,
            $"guilds/{guildId}/members/{userId}", body, ContentType.JsonBody, (ScopeType.Guild, guildId));

    public static IApiInOutRoute<ModifyGuildMemberParams, GuildMember> ModifyGuildMember(ulong guildId, ulong userId,
        ModifyGuildMemberParams body) =>
        new ApiInOutRoute<ModifyGuildMemberParams, GuildMember>(nameof(ModifyGuildMember), RequestMethod.Patch,
            $"guilds/{guildId}/members/{userId}", body, ContentType.JsonBody, (ScopeType.Guild, guildId));

    public static IApiInOutRoute<ModifyCurrentMemberParams, GuildMember> ModifyCurrentMember(ulong guildId,
        ModifyCurrentMemberParams body) =>
        new ApiInOutRoute<ModifyCurrentMemberParams, GuildMember>(nameof(ModifyCurrentMember), RequestMethod.Patch,
            $"guilds/{guildId}/members/@me", body, ContentType.JsonBody, (ScopeType.Guild, guildId));

    public static IApiRoute AddGuildMemberRole(ulong guildId, ulong userId, ulong roleId) =>
        new ApiRoute(nameof(AddGuildMemberRole), RequestMethod.Put,
            $"guilds/{guildId}/members/{userId}/roles/{roleId}", (ScopeType.Guild, guildId));

    public static IApiRoute RemoveGuildMemberRole(ulong guildId, ulong userId, ulong roleId) =>
        new ApiRoute(nameof(RemoveGuildMemberRole), RequestMethod.Delete,
            $"guilds/{guildId}/members/{userId}/roles/{roleId}", (ScopeType.Guild, guildId));

    public static IApiRoute RemoveGuildMember(ulong guildId, ulong userId) =>
        new ApiRoute(nameof(RemoveGuildMember), RequestMethod.Delete, $"guilds/{guildId}/members/{userId}",
            (ScopeType.Guild, guildId));

    public static IApiOutRoute<Ban[]> GetGuildBans(ulong guildId, int? limit = default, ulong? beforeId = default,
        ulong? afterId = default) =>
        new ApiOutRoute<Ban[]>(nameof(GetGuildBans), RequestMethod.Get,
            $"guilds/{guildId}/bans{RouteUtils.GetUrlEncodedQueryParams(("limit", limit), ("before", beforeId), ("after", afterId))}",
            (ScopeType.Guild, guildId));

    public static IApiOutRoute<Ban> GetGuildBan(ulong guildId, ulong userId) =>
        new ApiOutRoute<Ban>(nameof(GetGuildBan), RequestMethod.Get, $"guilds/{guildId}/bans/{userId}",
            (ScopeType.Guild, guildId));

    public static IApiInRoute<CreateGuildBanParams> CreateGuildBan(ulong guildId, ulong userId,
        CreateGuildBanParams body) =>
        new ApiInRoute<CreateGuildBanParams>(nameof(CreateGuildBan), RequestMethod.Put,
            $"guilds/{guildId}/bans/{userId}", body, ContentType.JsonBody, (ScopeType.Guild, guildId));

    public static IApiRoute RemoveGuildBan(ulong guildId, ulong userId) =>
        new ApiRoute(nameof(RemoveGuildBan), RequestMethod.Delete, $"guilds/{guildId}/bans/{userId}",
            (ScopeType.Guild, guildId));

    public static IApiInOutRoute<BulkBanUsersParams, BulkBanResponse> BulkGuildBan(ulong guildId,
        BulkBanUsersParams args) =>
        new ApiInOutRoute<BulkBanUsersParams, BulkBanResponse>(nameof(BulkGuildBan), RequestMethod.Post,
            $"guilds/{guildId}/bulk-ban", args, ContentType.JsonBody, (ScopeType.Guild, guildId));

    public static IApiOutRoute<Role[]> GetGuildRoles(ulong guildId) =>
        new ApiOutRoute<Role[]>(nameof(GetGuildRoles), RequestMethod.Get, $"guilds/{guildId}/roles",
            (ScopeType.Guild, guildId));

    public static IApiInOutRoute<CreateGuildRoleParams, Role>
        CreateGuildRole(ulong guildId, CreateGuildRoleParams body) =>
        new ApiInOutRoute<CreateGuildRoleParams, Role>(nameof(CreateGuildRole), RequestMethod.Post,
            $"guilds/{guildId}/roles", body, ContentType.JsonBody, (ScopeType.Guild, guildId));

    public static IApiInOutRoute<ModifyGuildRolePositionParams[], Role[]> ModifyGuildRolePositions(ulong guildId,
        ModifyGuildRolePositionParams[] body) =>
        new ApiInOutRoute<ModifyGuildRolePositionParams[], Role[]>(nameof(ModifyGuildRolePositions),
            RequestMethod.Patch, $"guilds/{guildId}/roles", body, ContentType.JsonBody, (ScopeType.Guild, guildId));

    public static IApiInOutRoute<ModifyGuildRoleParams, Role> ModifyGuildRole(ulong guildId, ulong roleId,
        ModifyGuildRoleParams body) =>
        new ApiInOutRoute<ModifyGuildRoleParams, Role>(nameof(ModifyGuildRole), RequestMethod.Patch,
            $"guilds/{guildId}/roles/{roleId}", body, ContentType.JsonBody, (ScopeType.Guild, guildId));

    public static IApiInOutRoute<ModifyGuildMfaLevelParams, MfaLevelResponse> ModifyGuildMfaLevel(ulong guildId,
        ModifyGuildMfaLevelParams body) =>
        new ApiInOutRoute<ModifyGuildMfaLevelParams, MfaLevelResponse>(nameof(ModifyGuildMfaLevel), RequestMethod.Patch,
            $"guilds/{guildId}/mfa", body, ContentType.JsonBody, (ScopeType.Guild, guildId));

    public static IApiRoute DeleteGuildRole(ulong guildId, ulong roleId) =>
        new ApiRoute(nameof(DeleteGuildRole), RequestMethod.Delete, $"guilds/{guildId}/roles/{roleId}",
            (ScopeType.Guild, guildId));

    public static IApiOutRoute<GuildPruneCount> GetGuildPruneCount(ulong guildId, int? days = default,
        ulong[]? includeRoles = default) =>
        new ApiOutRoute<GuildPruneCount>(nameof(GetGuildPruneCount), RequestMethod.Get,
            $"guilds/{guildId}/prune{RouteUtils.GetUrlEncodedQueryParams(("days", days), ("include_roles", includeRoles))}",
            (ScopeType.Guild, guildId));

    public static IApiInOutRoute<BeginGuildPruneParams, GuildPruneCount> BeginGuildPrune(ulong guildId,
        BeginGuildPruneParams body) =>
        new ApiInOutRoute<BeginGuildPruneParams, GuildPruneCount>(nameof(BeginGuildPrune), RequestMethod.Post,
            $"guilds/{guildId}/prune", body, ContentType.JsonBody, (ScopeType.Guild, guildId));

    public static IApiOutRoute<Models.Json.VoiceRegion[]> GetGuildVoiceRegions(ulong guildId) =>
        new ApiOutRoute<Models.Json.VoiceRegion[]>(nameof(GetGuildVoiceRegions), RequestMethod.Get,
            $"guilds/{guildId}/regions", (ScopeType.Guild, guildId));

    public static IApiOutRoute<InviteMetadata[]> GetGuildInvites(ulong guildId) =>
        new ApiOutRoute<InviteMetadata[]>(nameof(GetGuildInvites), RequestMethod.Get, $"guilds/{guildId}/invites",
            (ScopeType.Guild, guildId));

    public static IApiOutRoute<Models.Json.Integration[]> GetGuildIntegrations(ulong guildId) =>
        new ApiOutRoute<Models.Json.Integration[]>(nameof(GetGuildIntegrations), RequestMethod.Get,
            $"guilds/{guildId}/integrations", (ScopeType.Guild, guildId));

    public static IApiRoute DeleteGuildIntegration(ulong guildId, ulong integrationId) =>
        new ApiRoute(nameof(DeleteGuildIntegration), RequestMethod.Delete,
            $"guilds/{guildId}/integrations/{integrationId}", (ScopeType.Guild, guildId));

    public static IApiOutRoute<GuildWidgetSettings> GetGuildWidgetSettings(ulong guildId) =>
        new ApiOutRoute<GuildWidgetSettings>(nameof(GetGuildWidgetSettings), RequestMethod.Get,
            $"guilds/{guildId}/widget", (ScopeType.Guild, guildId));

    public static IApiInOutRoute<ModifyGuildWidgetParams, GuildWidgetSettings> ModifyGuildWidgetSettings(ulong guildId,
        ModifyGuildWidgetParams body) =>
        new ApiInOutRoute<ModifyGuildWidgetParams, GuildWidgetSettings>(nameof(ModifyGuildWidgetSettings),
            RequestMethod.Patch, $"guilds/{guildId}/widget", body, ContentType.JsonBody, (ScopeType.Guild, guildId));

    public static IApiOutRoute<GuildWidget> GetGuildWidget(ulong guildId) =>
        new ApiOutRoute<GuildWidget>(nameof(GetGuildWidget), RequestMethod.Get, $"guilds/{guildId}/widget.json",
            (ScopeType.Guild, guildId));

    public static IApiOutRoute<Invite> GetGuildVanityUrl(ulong guildId) =>
        new ApiOutRoute<Invite>(nameof(GetGuildVanityUrl), RequestMethod.Get, $"guilds/{guildId}/vanity-url",
            (ScopeType.Guild, guildId));

    public static IApiOutRoute<WelcomeScreen> GetGuildWelcomeScreen(ulong guildId) =>
        new ApiOutRoute<WelcomeScreen>(nameof(GetGuildWelcomeScreen), RequestMethod.Get,
            $"guilds/{guildId}/welcome-screen", (ScopeType.Guild, guildId));

    public static IApiInOutRoute<ModifyGuildWelcomeScreenParams, WelcomeScreen> ModifyGuildWelcomeScreen(ulong guildId,
        ModifyGuildWelcomeScreenParams body) =>
        new ApiInOutRoute<ModifyGuildWelcomeScreenParams, WelcomeScreen>(nameof(ModifyGuildWelcomeScreen),
            RequestMethod.Patch, $"guilds/{guildId}/welcome-screen", body, ContentType.JsonBody,
            (ScopeType.Guild, guildId));

    // TODO: add
    //public static string GuildOnboarding(ulong guildId)
    //        => $"guilds/{guildId}/onboarding";

    public static IApiInRoute<ModifyCurrentUserVoiceState> ModifyCurrentUserVoiceState(ulong guildId,
        ModifyCurrentUserVoiceState body) =>
        new ApiInRoute<ModifyCurrentUserVoiceState>(nameof(ModifyCurrentUserVoiceState), RequestMethod.Patch,
            $"guilds/{guildId}/voice-states/@me", body, ContentType.JsonBody, (ScopeType.Guild, guildId));

    public static IApiInRoute<ModifyUserVoiceState> ModifyUserVoiceState(ulong guildId, ulong userId,
        ModifyUserVoiceState body) =>
        new ApiInRoute<ModifyUserVoiceState>(nameof(ModifyUserVoiceState), RequestMethod.Patch,
            $"guilds/{guildId}/voice-states/{userId}", body, ContentType.JsonBody, (ScopeType.Guild, guildId));
}
