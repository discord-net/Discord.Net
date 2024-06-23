//public static string Guilds
//    => "guilds";

//public static string GetGuild(ulong guildId, bool? withCounts = default)
//        => $"guilds/{guildId}{GetUrlEncodedQueryParams(("with_counts", withCounts))}";

//public static string GetGuildPreview(ulong guildId)
//        => $"guilds/{guildId}/preview";

//public static string UpdateGuild(ulong guildId)
//        => $"guilds/{guildId}";

//public static string GuildChannels(ulong guildId)
//        => $"guilds/{guildId}/channels";

//public static string ListActiveThreads(ulong guildId)
//        => $"guilds/{guildId}/threads/active";

//public static string GuildMember(ulong guildId, ulong userId)
//        => $"guilds/{guildId}/members/{userId}";

//public static string ListGuildMembers(ulong guildId, int? limit = default, ulong? afterId = default)
//        => $"guilds/{guildId}/members{GetUrlEncodedQueryParams(("limit", limit),
//            ("after", afterId))}";

//public static string SearchGuildMembers(ulong guildId, string query, int? limit = default)
//        => $"guilds/{guildId}/members/search{GetUrlEncodedQueryParams(("query", query),
//            ("limit", limit))}";

//public static string ModifyCurrentMember(ulong guildId)
//        => $"guilds/{guildId}/members/@me";

//public static string GuildMemberRole(ulong guildId, ulong userId, ulong roleId)
//        => $"guilds/{guildId}/members/{userId}/roles/{roleId}";

//public static string GetGuildBans(ulong guildId, int? limit = default, ulong? beforeId = default, ulong? afterId = default)
//        => $"guilds/{guildId}/bans{GetUrlEncodedQueryParams(("limit", limit),
//            ("before", beforeId),
//            ("after", afterId))}";

//public static string GuildBan(ulong guildId, ulong userId)
//        => $"guilds/{guildId}/bans/{userId}";

//public static string GuildRoles(ulong guildId)
//        => $"guilds/{guildId}/roles";

//public static string GuildRole(ulong guildId, ulong roleId)
//        => $"guilds/{guildId}/roles/{roleId}";

//public static string GuildMfaLevel(ulong guildId)
//        => $"guilds/{guildId}/mfa";

//public static string GetGuildPruneCount(ulong guildId, int days = 7, ulong[]? includeRoles = default)
//        => $"guilds/{guildId}/prune?days={days}{(includeRoles is not null && includeRoles.Length > 0
//            ? "&include_roles=" + GetCommaDelimitedSnowflakeArray(includeRoles)
//            : string.Empty)}";

//public static string BeginGuildPrune(ulong guildId)
//        => $"guilds/{guildId}/prune";

//public static string GuildVoiceRegions(ulong guildId)
//        => $"guilds/{guildId}/regions";

//public static string GuildInvites(ulong guildId)
//        => $"guilds/{guildId}/invites";

//public static string GuildIntegrations(ulong guildId)
//        => $"guilds/{guildId}/integrations";

//public static string DeleteIntegration(ulong guildId, ulong integrationId)
//        => $"guilds/{guildId}/integrations/{integrationId}";

//public static string GuildWidget(ulong guildId)
//        => $"guilds/{guildId}/widget";

//public static string GetWidgetJson(ulong guildId)
//        => $"guilds/{guildId}/widget.json";

//public static string GuildVanityUrl(ulong guildId)
//        => $"guilds/{guildId}/vanity-url";

//public static string GuildWidgetImage(ulong guildId, string? style = default)
//        => $"guilds/{guildId}/widget.png{GetUrlEncodedQueryParams(("style", style))}";

//public static string GuildWelcomeScreen(ulong guildId)
//        => $"guilds/{guildId}/welcome-screen";

//public static string GuildOnboarding(ulong guildId)
//        => $"guilds/{guildId}/onboarding";

//public static string ModifyCurrentUserVoiceState(ulong guildId)
//        => $"guilds/{guildId}/voice-states/@me";

//public static string ModifyUserVoiceState(ulong guildId, ulong userId)
//        => $"guilds/{guildId}/voice-states/{userId}";



