using Discord.Models.Json;
using Discord.Utils;

namespace Discord.Rest;

public partial class Routes
{
    public static ApiBodyRoute<CreateGuildParams, Guild> CreateGuild(CreateGuildParams body)
        => new(nameof(CreateGuild),
            RequestMethod.Post,
            "guilds",
            body);

    public static ApiRoute<Guild> GetGuild(ulong guildId, bool? withCounts = default)
        => new(nameof(GetGuild),
            RequestMethod.Get,
            $"guilds/{guildId}{RouteUtils.GetUrlEncodedQueryParams(("with_counts", withCounts))}");

    public static ApiRoute<GuildPreview> GetGuildPreview(ulong guildId)
        => new(nameof(GetGuildPreview),
            RequestMethod.Get,
            $"guilds/{guildId}/preview",
            (ScopeType.Guild, guildId));

    public static ApiBodyRoute<ModifyGuildParams, Guild> ModifyGuild(ulong guildId, ModifyGuildParams body)
        => new(nameof(ModifyGuild),
            RequestMethod.Patch,
            $"guilds/{guildId}",
            body,
            ContentType.JsonBody,
            (ScopeType.Guild, guildId));

    public static BasicApiRoute DeleteGuild(ulong guildId)
        => new(nameof(DeleteGuild),
            RequestMethod.Delete,
            $"guilds/{guildId}",
            (ScopeType.Guild, guildId));

    public static ApiRoute<Channel[]> GetGuildChannels(ulong guildId)
        => new(nameof(GetGuildChannels),
            RequestMethod.Get,
            $"guilds/{guildId}/channels",
            (ScopeType.Guild, guildId));

    public static ApiBodyRoute<CreateGuildChannelParams, GuildChannelBase> CreateGuildChannel(ulong guildId,
        CreateGuildChannelParams body)
        => new(nameof(CreateGuildChannel),
            RequestMethod.Post,
            $"guilds/{guildId}/channels",
            body,
            ContentType.JsonBody,
            (ScopeType.Guild, guildId)
        );

    public static ApiBodyRoute<ModifyGuildChannelPositionsParams[]> ModifyGuildChannelPositions(ulong guildId,
        ModifyGuildChannelPositionsParams[] body)
        => new(nameof(ModifyGuildChannelPositions),
            RequestMethod.Patch,
            $"guild/{guildId}/channels",
            body,
            ContentType.JsonBody,
            (ScopeType.Guild, guildId));

    public static ApiRoute<ListActiveGuildThreadsResponse> ListActiveGuildThreads(ulong guildId)
        => new(nameof(ListActiveGuildThreads),
            RequestMethod.Get,
            $"guilds/{guildId}/threads/active",
            (ScopeType.Guild, guildId));

    public static ApiRoute<GuildMember> GetGuildMember(ulong guildId, ulong userId)
        => new(nameof(GetGuildMember),
            RequestMethod.Get,
            $"guilds/{guildId}/members/{userId}",
            (ScopeType.Guild, guildId));

    public static ApiRoute<GuildMember[]> ListGuildMembers(ulong guildId, int? limit = default,
        ulong? afterId = default)
        => new(nameof(ListGuildMembers),
            RequestMethod.Get,
            $"guilds/{guildId}/members{RouteUtils.GetUrlEncodedQueryParams(("limit", limit), ("after", afterId))}",
            (ScopeType.Guild, guildId));

    public static ApiRoute<GuildMember[]> SearchGuildMembers(ulong guildId, string query, int? limit = default)
        => new(nameof(SearchGuildMembers),
            RequestMethod.Get,
            $"guilds/{guildId}/members/search{RouteUtils.GetUrlEncodedQueryParams(("query", query), ("limit", limit))}",
            (ScopeType.Guild, guildId));

    public static ApiBodyRoute<AddGuildMemberParams, GuildMember> AddGuildMember(ulong guildId, ulong userId,
        AddGuildMemberParams body)
        => new(nameof(AddGuildMember),
            RequestMethod.Put,
            $"guilds/{guildId}/members/{userId}",
            body,
            ContentType.JsonBody,
            (ScopeType.Guild, guildId));

    public static ApiBodyRoute<ModifyGuildMemberParams, GuildMember> ModifyGuildMember(ulong guildId, ulong userId,
        ModifyGuildMemberParams body)
        => new(nameof(ModifyGuildMember),
            RequestMethod.Patch,
            $"guilds/{guildId}/members/{userId}",
            body,
            ContentType.JsonBody,
            (ScopeType.Guild, guildId));

    public static ApiBodyRoute<ModifyCurrentMemberParams, GuildMember> ModifyCurrentMember(ulong guildId,
        ModifyCurrentMemberParams body)
        => new(nameof(ModifyCurrentMember),
            RequestMethod.Patch,
            $"guilds/{guildId}/members/@me",
            body,
            ContentType.JsonBody,
            (ScopeType.Guild, guildId));

    public static BasicApiRoute AddGuildMemberRole(ulong guildId, ulong userId, ulong roleId)
        => new(nameof(AddGuildMemberRole),
            RequestMethod.Put,
            $"guilds/{guildId}/members/{userId}/roles/{roleId}",
            (ScopeType.Guild, guildId));

    public static BasicApiRoute RemoveGuildMemberRole(ulong guildId, ulong userId, ulong roleId)
        => new(nameof(RemoveGuildMemberRole),
            RequestMethod.Delete,
            $"guilds/{guildId}/members/{userId}/roles/{roleId}",
            (ScopeType.Guild, guildId));

    public static BasicApiRoute RemoveGuildMember(ulong guildId, ulong userId)
        => new(nameof(RemoveGuildMember),
            RequestMethod.Delete,
            $"guilds/{guildId}/members/{userId}",
            (ScopeType.Guild, guildId));

    public static ApiRoute<Ban[]> GetGuildBans(ulong guildId, int? limit = default, ulong? beforeId = default,
        ulong? afterId = default)
        => new(nameof(GetGuildBans),
            RequestMethod.Get,
            $"guilds/{guildId}/bans{RouteUtils.GetUrlEncodedQueryParams(("limit", limit), ("before", beforeId), ("after", afterId))}",
            (ScopeType.Guild, guildId));

    public static ApiRoute<Ban> GetGuildBan(ulong guildId, ulong userId)
        => new(nameof(GetGuildBan),
            RequestMethod.Get,
            $"guilds/{guildId}/bans/{userId}",
            (ScopeType.Guild, guildId));

    public static ApiBodyRoute<CreateGuildBanParams> CreateGuildBan(ulong guildId, ulong userId,
        CreateGuildBanParams body)
        => new(nameof(CreateGuildBan),
            RequestMethod.Put,
            $"guilds/{guildId}/bans/{userId}",
            body,
            ContentType.JsonBody,
            (ScopeType.Guild, guildId));

    public static BasicApiRoute RemoveGuildBan(ulong guildId, ulong userId)
        => new(nameof(RemoveGuildBan),
            RequestMethod.Delete,
            $"guilds/{guildId}/bans/{userId}",
            (ScopeType.Guild, guildId));

    public static ApiRoute<Role[]> GetGuildRoles(ulong guildId)
        => new(nameof(GetGuildRoles),
            RequestMethod.Get,
            $"guilds/{guildId}/roles",
            (ScopeType.Guild, guildId));

    public static ApiBodyRoute<CreateGuildRoleParams, Role> CreateGuildRole(ulong guildId, CreateGuildRoleParams body)
        => new(nameof(CreateGuildRole),
            RequestMethod.Post,
            $"guilds/{guildId}/roles",
            body,
            ContentType.JsonBody,
            (ScopeType.Guild, guildId));

    public static ApiBodyRoute<ModifyGuildRolePositionParams[], Role[]> ModifyGuildRolePositions(ulong guildId,
        ModifyGuildRolePositionParams[] body)
        => new(nameof(ModifyGuildRolePositions),
            RequestMethod.Patch,
            $"guilds/{guildId}/roles",
            body,
            ContentType.JsonBody,
            (ScopeType.Guild, guildId));

    public static ApiBodyRoute<ModifyGuildRoleParams, Role> ModifyGuildRole(ulong guildId, ulong roleId,
        ModifyGuildRoleParams body)
        => new(nameof(ModifyGuildRole),
            RequestMethod.Patch,
            $"guilds/{guildId}/roles/{roleId}",
            body,
            ContentType.JsonBody,
            (ScopeType.Guild, guildId));

    public static ApiBodyRoute<ModifyGuildMfaLevelParams, MfaLevelResponse> ModifyGuildMfaLevel(ulong guildId,
        ModifyGuildMfaLevelParams body)
        => new(nameof(ModifyGuildMfaLevel),
            RequestMethod.Patch,
            $"guilds/{guildId}/mfa",
            body,
            ContentType.JsonBody,
            (ScopeType.Guild, guildId));

    public static BasicApiRoute DeleteGuildRole(ulong guildId, ulong roleId)
        => new(nameof(DeleteGuildRole),
            RequestMethod.Delete,
            $"guilds/{guildId}/roles/{roleId}",
            (ScopeType.Guild, guildId));

    public static ApiRoute<GuildPruneCount> GetGuildPruneCount(ulong guildId, int? days = default,
        ulong[]? includeRoles = default)
        => new(nameof(GetGuildPruneCount),
            RequestMethod.Get,
            $"guilds/{guildId}/prune{RouteUtils.GetUrlEncodedQueryParams(("days", days), ("include_roles", includeRoles))}",
            (ScopeType.Guild, guildId));

    public static ApiBodyRoute<BeginGuildPruneParams, GuildPruneCount> BeginGuildPrune(ulong guildId,
        BeginGuildPruneParams body)
        => new(nameof(BeginGuildPrune),
            RequestMethod.Post,
            $"guilds/{guildId}/prune",
            body,
            ContentType.JsonBody,
            (ScopeType.Guild, guildId));

    public static ApiRoute<VoiceRegion[]> GetGuildVoiceRegions(ulong guildId)
        => new(nameof(GetGuildVoiceRegions),
            RequestMethod.Get,
            $"guilds/{guildId}/regions",
            (ScopeType.Guild, guildId));

    public static ApiRoute<InviteMetadata[]> GetGuildInvites(ulong guildId)
        => new(nameof(GetGuildInvites),
            RequestMethod.Get,
            $"guilds/{guildId}/invites",
            (ScopeType.Guild, guildId));

    public static ApiRoute<Integration[]> GetGuildIntegrations(ulong guildId)
        => new(nameof(GetGuildIntegrations),
            RequestMethod.Get,
            $"guilds/{guildId}/integrations",
            (ScopeType.Guild, guildId));

    public static BasicApiRoute DeleteGuildIntegration(ulong guildId, ulong integrationId)
        => new(nameof(DeleteGuildIntegration),
            RequestMethod.Delete,
            $"guilds/{guildId}/integrations/{integrationId}",
            (ScopeType.Guild, guildId));

    public static ApiRoute<GuildWidgetSettings> GetGuildWidgetSettings(ulong guildId)
        => new(nameof(GetGuildWidgetSettings),
            RequestMethod.Get,
            $"guilds/{guildId}/widget",
            (ScopeType.Guild, guildId));

    public static ApiBodyRoute<ModifyGuildWidgetParams, GuildWidgetSettings> ModifyGuildWidgetSettings(ulong guildId,
        ModifyGuildWidgetParams body)
        => new(nameof(ModifyGuildWidgetSettings),
            RequestMethod.Patch,
            $"guilds/{guildId}/widget",
            body,
            ContentType.JsonBody,
            (ScopeType.Guild, guildId));

    public static ApiRoute<GuildWidget> GetGuildWidget(ulong guildId)
        => new(nameof(GetGuildWidget),
            RequestMethod.Get,
            $"guilds/{guildId}/widget.json",
            (ScopeType.Guild, guildId));

    public static ApiRoute<Invite> GetGuildVanityUrl(ulong guildId)
        => new(nameof(GetGuildVanityUrl),
            RequestMethod.Get,
            $"guilds/{guildId}/vanity-url",
            (ScopeType.Guild, guildId));

    public static ApiRoute<WelcomeScreen> GetGuildWelcomeScreen(ulong guildId)
        => new(nameof(GetGuildWelcomeScreen),
            RequestMethod.Get,
            $"guilds/{guildId}/welcome-screen",
            (ScopeType.Guild, guildId));

    public static ApiBodyRoute<ModifyGuildWelcomeScreenParams, WelcomeScreen> ModifyGuildWelcomeScreen(ulong guildId,
        ModifyGuildWelcomeScreenParams body)
        => new(nameof(ModifyGuildWelcomeScreen),
            RequestMethod.Patch,
            $"guilds/{guildId}/welcome-screen",
            body,
            ContentType.JsonBody,
            (ScopeType.Guild, guildId));

    // TODO: add
    //public static string GuildOnboarding(ulong guildId)
    //        => $"guilds/{guildId}/onboarding";

    public static ApiBodyRoute<ModifyCurrentUserVoiceState> ModifyCurrentUserVoiceState(ulong guildId,
        ModifyCurrentUserVoiceState body)
        => new(nameof(ModifyCurrentUserVoiceState),
            RequestMethod.Patch,
            $"guilds/{guildId}/voice-states/@me",
            body,
            ContentType.JsonBody,
            (ScopeType.Guild, guildId));

    public static ApiBodyRoute<ModifyUserVoiceState> ModifyUserVoiceState(ulong guildId, ulong userId,
        ModifyUserVoiceState body)
        => new(nameof(ModifyUserVoiceState),
            RequestMethod.Patch,
            $"guilds/{guildId}/voice-states/{userId}",
            body,
            ContentType.JsonBody,
            (ScopeType.Guild, guildId));
}
