//using System.Net;

//namespace Discord;

///// <summary>
/////     Generates route strings for Discord API endpoints.
///// </summary>
//public class Routes
//{
//    //public static string GetUrlEncodedQueryParams(params (string, object?)[] args)
//    //{
//    //    if (args.All(x => x.Item2 is null))
//    //        return string.Empty;

//    //    var paramsString = string.Join("&", args.Where(x => x.Item2 is not null)
//    //        .Select(x => GetUrlEncodedQueryParam(x.Item1, x.Item2!)));

//    //    return $"?{paramsString}";
//    //}

//    //public static string GetUrlEncodedQueryParam(string key, object value)
//    //    => $"{key}={WebUtility.UrlEncode(value.ToString())}";

//    //public static string GetCommaDelimitedSnowflakeArray(ulong[] ids)
//    //    => ids.Length > 0
//    //        ? string.Join(',', ids)
//    //        : string.Empty;


//    //#region Applications

//    //public static string CurrentApplication
//    //    => $"applications/@me";

//    //#endregion

//    //#region Application Role Connection Metadata

//    //public static string ApplicationRoleConnectionMetadataRecords(ulong applicationId)
//    //    => $"applications/{applicationId}/role-connections/metadata";

//    //#endregion

//    //#region Audit Log

//    //public static string GetAuditLog(ulong guildId)
//    //    => $"guilds/{guildId}/audit-logs";

//    //#endregion

//    //#region Auto Moderation

//    //public static string GuildAutoModerationRules(ulong guildId)
//    //    => $"guilds/{guildId}/auto-moderation/rules";

//    //public static string GuildAutoModerationRule(ulong guildId, ulong ruleId)
//    //    => $"guilds/{guildId}/auto-moderation/rules/{ruleId}";

//    //#endregion

//    //#region Channel

//    //public static string Channel(ulong channelId)
//    //    => $"/channels/{channelId}";

//    //public static string GetMessages(ulong channelId, ulong? aroundId = default, ulong? beforeId = default, ulong? afterId = default, int? limit = default)
//    //    => $"/channels/{channelId}/messages{GetUrlEncodedQueryParams(("around", aroundId),
//    //        ("before", beforeId),
//    //        ("after", afterId),
//    //        ("limit", limit))}";

//    //public static string CreateMessage(ulong channelId)
//    //    => $"/channels/{channelId}";

//    //public static string CrosspostMessage(ulong channelId, ulong messageId)
//    //    => $"/channels/{channelId}/messages/{messageId}/crosspost";

//    //public static string MessageOwnReaction(ulong channelId, ulong messageId, string emoji)
//    //    => $"/channels/{channelId}/messages/{messageId}/reactions/{WebUtility.UrlEncode(emoji)}/@me";

//    //public static string DeleteUserReaction(ulong channelId, ulong messageId, string emoji, ulong userId)
//    //    => $"/channels/{channelId}/messages/{messageId}/reactions/{WebUtility.UrlEncode(emoji)}/{userId}";

//    //public static string GetReactions(ulong channelId, ulong messageId, string emoji, ulong? afterId = default, int? limit = default)
//    //    => $"/channels/{channelId}/messages/{messageId}/reactions/{WebUtility.UrlEncode(emoji)}{GetUrlEncodedQueryParams(("after", afterId), ("limit", limit))}";

//    //public static string DeleteAllReactions(ulong channelId, ulong messageId)
//    //    => $"/channels/{channelId}/messages/{messageId}/reactions";

//    //public static string DeleteAllReactionsForEmoji(ulong channelId, ulong messageId, string emoji)
//    //    => $"/channels/{channelId}/messages/{messageId}/reactions/{WebUtility.UrlEncode(emoji)}";

//    //public static string UpdateMessage(ulong channelId, ulong messageId)
//    //    => $"/channels/{channelId}/messages/{messageId}";

//    //public static string BulkDeleteMessages(ulong channelId)
//    //    => $"/channels/{channelId}/messages";

//    //public static string ChannelPermissions(ulong channelId, ulong overwriteId)
//    //    => $"/channels/{channelId}/permissions/{overwriteId}";

//    //public static string ChannelInvites(ulong channelId)
//    //    => $"/channels/{channelId}/invites";

//    //public static string FollowAnnouncementChannel(ulong channelId)
//    //    => $"/channels/{channelId}/followers";

//    //public static string TriggerTyping(ulong channelId)
//    //    => $"/channels/{channelId}/typing";

//    //public static string PinnedMessages(ulong channelId)
//    //    => $"/channels/{channelId}/pins";

//    //public static string UpdateChannelPins(ulong channelId, ulong messageId)
//    //    => $"/channels/{channelId}/pins/{messageId}";

//    //public static string GroupDmRecipients(ulong channelId, ulong userId)
//    //    => $"/channels/{channelId}/recipients/{userId}";

//    //public static string StartThreadFromMessage(ulong channelId, ulong messageId)
//    //    => $"/channels/{channelId}/messages/{messageId}/threads";

//    //public static string StartThreadWithoutMessage(ulong channelId)
//    //    => $"/channels/{channelId}/threads";

//    //public static string ManageOwnThreadMember(ulong channelId)
//    //    => $"/channels/{channelId}/thread-members/@me";

//    //public static string ManageThreadMember(ulong channelId, ulong userId)
//    //    => $"/channels/{channelId}/thread-members/{userId}";

//    //public static string GetThreadMember(ulong channelId, ulong userId, bool? withMember = default)
//    //    => $"/channels/{channelId}/thread-members/{userId}{GetUrlEncodedQueryParams(("with_member", withMember))}";

//    //public static string ListThreadMembers(ulong channelId, bool? withMember = default, ulong? afterId = default, int? limit = default)
//    //    => $"/channels/{channelId}/thread-members{GetUrlEncodedQueryParams(("with_member", withMember),
//    //        ("after", afterId),
//    //        ("limit", limit))}";

//    //public static string ListPublicArchivedThreads(ulong channelId, DateTimeOffset? before = default, int? limit = default)
//    //    => $"/channels/{channelId}/threads/archived/public{GetUrlEncodedQueryParams(("before", before),
//    //        ("limit", limit))}";

//    //public static string ListPrivateArchivedThreads(ulong channelId, DateTimeOffset? before = default, int? limit = default)
//    //    => $"/channels/{channelId}/threads/archived/private{GetUrlEncodedQueryParams(("before", before),
//    //        ("limit", limit))}";

//    //public static string ListPrivateArchivedThreads(ulong channelId, ulong? beforeId = default, int? limit = default)
//    //    => $"/channels/{channelId}/users/@me/threads/archived/private{GetUrlEncodedQueryParams(("before", beforeId),
//    //        ("limit", limit))}";

//    //#endregion

//    //#region Emoji

//    //public static string GuildEmojis(ulong guildId)
//    //    => $"guilds/{guildId}/emojis";

//    //public static string Emoji(ulong guildId, ulong emojiId)
//    //    => $"guilds/{guildId}/emojis/{emojiId}";

//    //#endregion

//    //#region Guild

//    //public static string Guilds
//    //    => "guilds";

//    //public static string GetGuild(ulong guildId, bool? withCounts = default)
//    //        => $"guilds/{guildId}{GetUrlEncodedQueryParams(("with_counts", withCounts))}";

//    //public static string GetGuildPreview(ulong guildId)
//    //        => $"guilds/{guildId}/preview";

//    //public static string UpdateGuild(ulong guildId)
//    //        => $"guilds/{guildId}";

//    //public static string GuildChannels(ulong guildId)
//    //        => $"guilds/{guildId}/channels";

//    //public static string ListActiveThreads(ulong guildId)
//    //        => $"guilds/{guildId}/threads/active";

//    //public static string GuildMember(ulong guildId, ulong userId)
//    //        => $"guilds/{guildId}/members/{userId}";

//    //public static string ListGuildMembers(ulong guildId, int? limit = default, ulong? afterId = default)
//    //        => $"guilds/{guildId}/members{GetUrlEncodedQueryParams(("limit", limit),
//    //            ("after", afterId))}";

//    //public static string SearchGuildMembers(ulong guildId, string query, int? limit = default)
//    //        => $"guilds/{guildId}/members/search{GetUrlEncodedQueryParams(("query", query),
//    //            ("limit", limit))}";

//    //public static string ModifyCurrentMember(ulong guildId)
//    //        => $"guilds/{guildId}/members/@me";

//    //public static string GuildMemberRole(ulong guildId, ulong userId, ulong roleId)
//    //        => $"guilds/{guildId}/members/{userId}/roles/{roleId}";

//    //public static string GetGuildBans(ulong guildId, int? limit = default, ulong? beforeId = default, ulong? afterId = default)
//    //        => $"guilds/{guildId}/bans{GetUrlEncodedQueryParams(("limit", limit),
//    //            ("before", beforeId),
//    //            ("after", afterId))}";

//    //public static string GuildBan(ulong guildId, ulong userId)
//    //        => $"guilds/{guildId}/bans/{userId}";

//    //public static string GuildRoles(ulong guildId)
//    //        => $"guilds/{guildId}/roles";

//    //public static string GuildRole(ulong guildId, ulong roleId)
//    //        => $"guilds/{guildId}/roles/{roleId}";

//    //public static string GuildMfaLevel(ulong guildId)
//    //        => $"guilds/{guildId}/mfa";

//    //public static string GetGuildPruneCount(ulong guildId, int days = 7, ulong[]? includeRoles = default)
//    //        => $"guilds/{guildId}/prune?days={days}{(includeRoles is not null && includeRoles.Length > 0
//    //            ? "&include_roles=" + GetCommaDelimitedSnowflakeArray(includeRoles)
//    //            : string.Empty)}";

//    //public static string BeginGuildPrune(ulong guildId)
//    //        => $"guilds/{guildId}/prune";

//    //public static string GuildVoiceRegions(ulong guildId)
//    //        => $"guilds/{guildId}/regions";

//    //public static string GuildInvites(ulong guildId)
//    //        => $"guilds/{guildId}/invites";

//    //public static string GuildIntegrations(ulong guildId)
//    //        => $"guilds/{guildId}/integrations";

//    //public static string DeleteIntegration(ulong guildId, ulong integrationId)
//    //        => $"guilds/{guildId}/integrations/{integrationId}";

//    //public static string GuildWidget(ulong guildId)
//    //        => $"guilds/{guildId}/widget";

//    //public static string GetWidgetJson(ulong guildId)
//    //        => $"guilds/{guildId}/widget.json";

//    //public static string GuildVanityUrl(ulong guildId)
//    //        => $"guilds/{guildId}/vanity-url";

//    //public static string GuildWidgetImage(ulong guildId, string? style = default)
//    //        => $"guilds/{guildId}/widget.png{GetUrlEncodedQueryParams(("style", style))}";

//    //public static string GuildWelcomeScreen(ulong guildId)
//    //        => $"guilds/{guildId}/welcome-screen";

//    //public static string GuildOnboarding(ulong guildId)
//    //        => $"guilds/{guildId}/onboarding";

//    //public static string ModifyCurrentUserVoiceState(ulong guildId)
//    //        => $"guilds/{guildId}/voice-states/@me";

//    //public static string ModifyUserVoiceState(ulong guildId, ulong userId)
//    //        => $"guilds/{guildId}/voice-states/{userId}";

//    //#endregion

//    //#region Guild Scheduled Events

//    //public static string ListGuildScheduledEvents(ulong guildId, bool? withUserCount = default)
//    //    => $"guilds/{guildId}/scheduled-events{GetUrlEncodedQueryParams(("with_user_count", withUserCount))}";

//    //public static string CreateGuildScheduledEvent(ulong guildId)
//    //    => $"guilds/{guildId}/scheduled-events";

//    //public static string GetGuildScheduledEvent(ulong guildId, ulong eventId, bool? withUserCount = default)
//    //    => $"guilds/{guildId}/scheduled-events/{eventId}{GetUrlEncodedQueryParams(("with_user_count", withUserCount))}";

//    //public static string UpdateGuildScheduledEvent(ulong guildId, ulong eventId)
//    //    => $"guilds/{guildId}/scheduled-events/{eventId}";

//    //public static string GetGuildScheduledEventUsers(ulong guildId, ulong eventId, int? limit = default, bool? withMember = default, ulong? beforeId = default, ulong? afterId = default)
//    //    => $"guilds/{guildId}/scheduled-events/{eventId}/users{GetUrlEncodedQueryParams(("limit", limit),
//    //        ("with_member", withMember),
//    //        ("before", beforeId),
//    //        ("after", afterId))}";

//    //#endregion

//    //#region Guild Template

//    //public static string GuildTemplate(string templateCode)
//    //    => $"guilds/templates/{templateCode}";

//    //public static string GuildTemplates(ulong guildId)
//    //    => $"guilds/{guildId}/templates";

//    //public static string UpdateGuildTemplate(ulong guildId, string templateCode)
//    //    => $"guilds/{guildId}/templates/{templateCode}";

//    //#endregion

//    //#region Invite

//    //public static string GetInvite(string code, bool? withCounts = default, bool? withExpiration = default, ulong? eventId = default)
//    //    => $"invites/{code}{GetUrlEncodedQueryParams(("with_counts", withCounts),
//    //        ("with_expiration", withExpiration),
//    //        ("guild_scheduled_event_id", eventId))}";

//    //public static string DeleteInvite(string code)
//    //    => $"invites/{code}";

//    //#endregion

//    //#region Stage Instances

//    //public static string CreateStageInstance
//    //    => $"stage-instances";

//    //public static string StageInstance(ulong instanceId)
//    //    => $"stage-instances/{instanceId}";

//    //#endregion

//    //#region Sticker

//    //public static string GetSticker(ulong stickerId)
//    //    => $"stickers/{stickerId}";

//    //public static string ListSticketPacks
//    //    => $"sticker-packs";

//    //public static string GuildStickers(ulong guildId)
//    //    => $"guilds/{guildId}/stickers";

//    //public static string GuildSticker(ulong guildId, ulong stickerId)
//    //    => $"guilds/{guildId}/stickers/{stickerId}";

//    //#endregion

//    //#region User

//    //public static string GetUser(ulong userId)
//    //    => $"users/{userId}";

//    //public static string CurrentUser
//    //    => $"users/@me";

//    //public static string GetCurrentUserGuilds(ulong? before = default, ulong? after = default, int? limit = default, bool? withCounts = default)
//    //    => $"users/@me/guilds{GetUrlEncodedQueryParams(("before", before),
//    //        ("after", after),
//    //        ("limit", limit),
//    //        ("with_counts", withCounts))}";

//    //public static string GetCurrentUserGuildMember(ulong guildId)
//    //    => $"users/@me/guilds/{guildId}/member";

//    //public static string LeaveGuild(ulong guildId)
//    //    => $"users/@me/guilds/{guildId}";

//    //public static string CreateDm
//    //    => "users/@me/channels";

//    //public static string GetUserConnections
//    //    => "users/@me/connections";

//    //public static string ApplicationRoleConnection(ulong applicationId)
//    //    => $"users/@me/applications/{applicationId}/role-connection";

//    //#endregion

//    //#region Voice

//    //public static string ListVoiceRegions
//    //    => $"voice/regions";

//    //#endregion

//    //#region Webhook

//    //public static string ChannelWebhook(ulong channelId)
//    //    => $"channels/{channelId}/webhooks";

//    //public static string GuildWebhooks(ulong guildId)
//    //    => $"guilds/{guildId}/webhooks";

//    //public static string Webhook(ulong webhookId)
//    //    => $"webhooks/{webhookId}";

//    //public static string Webhook(ulong webhookId, string webhookToken)
//    //    => $"webhooks/{webhookId}/{webhookToken}";

//    //public static string ExecuteWebhook(ulong webhookId, string webhookToken, bool? wait = default, ulong? threadId = default)
//    //    => $"webhooks/{webhookId}/{webhookToken}{GetUrlEncodedQueryParams(("wait", wait), ("thread_id", threadId))}";

//    //public static string ExecuteSlackWebhook(ulong webhookId, string webhookToken, bool? wait = default, ulong? threadId = default)
//    //    => $"webhooks/{webhookId}/{webhookToken}/slack{GetUrlEncodedQueryParams(("wait", wait), ("thread_id", threadId))}";

//    //public static string ExecuteGithubWebhook(ulong webhookId, string webhookToken, bool? wait = default, ulong? threadId = default)
//    //    => $"webhooks/{webhookId}/{webhookToken}/github{GetUrlEncodedQueryParams(("wait", wait), ("thread_id", threadId))}";

//    //public static string WebhookMessage(ulong webhookId, string webhookToken, ulong messageId, ulong? threadId = default)
//    //    => $"webhooks/{webhookId}/{webhookToken}/messages/{messageId}{GetUrlEncodedQueryParams(("thread_id", threadId))}";

//    //#endregion
//}
