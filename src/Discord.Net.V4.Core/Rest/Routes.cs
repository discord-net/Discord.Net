using System.Net;

namespace Discord;

/// <summary>
///     Generates route strings for Discord API endpoints.
/// </summary>
public class Routes
{
    public static string GetUrlEncodedQueryParams(params (string, object?)[] args)
    {
        if (args.All(x => x.Item2 is null))
            return string.Empty;

        var paramsString = string.Join("&", args.Where(x => x.Item2 is not null)
            .Select(x => GetUrlEncodedQueryParam(x.Item1, x.Item2!)));

        return $"?{paramsString}";
    }

    public static string GetUrlEncodedQueryParam(string key, object value)
        => $"{key}={WebUtility.UrlEncode(value.ToString())}";


    #region Applications

    public static string CurrentApplication
        => $"applications/@me";

    #endregion

    #region Application Role Connection Metadata

    public static string ApplicationRoleConnectionMetadataRecords(ulong applicationId)
        => $"applications/{applicationId}/role-connections/metadata";

    #endregion

    #region Audit Log

    public static string GetAuditLog(ulong guildId)
        => $"guilds/{guildId}/audit-logs";

    #endregion

    #region Auto Moderation

    public static string GuildAutoModerationRules(ulong guildId)
        => $"guilds/{guildId}/auto-moderation/rules";

    public static string GuildAutoModerationRule(ulong guildId, ulong ruleId)
        => $"guilds/{guildId}/auto-moderation/rules/{ruleId}";

    #endregion

    #region Channel


    #endregion

    #region Emoji

    public static string GuildEmojis(ulong guildId)
        => $"guilds/{guildId}/emojis";

    public static string Emoji(ulong guildId, ulong emojiId)
        => $"guilds/{guildId}/emojis/{emojiId}";

    #endregion

    #region Guild


    #endregion

    #region Guild Scheduled Events

    public static string ListGuildScheduledEvents(ulong guildId, bool? withUserCount = default)
        => $"guilds/{guildId}/scheduled-events{GetUrlEncodedQueryParams(("with_user_count", withUserCount))}";

    public static string CreateGuildScheduledEvent(ulong guildId)
        => $"guilds/{guildId}/scheduled-events";

    public static string GetGuildScheduledEvent(ulong guildId, ulong eventId, bool? withUserCount = default)
        => $"guilds/{guildId}/scheduled-events/{eventId}{GetUrlEncodedQueryParams(("with_user_count", withUserCount))}";

    public static string UpdateGuildScheduledEvent(ulong guildId, ulong eventId)
        => $"guilds/{guildId}/scheduled-events/{eventId}";

    public static string GetGuildScheduledEventUsers(ulong guildId, ulong eventId, int? limit = default, bool? withMember = default, ulong? beforeId = default, ulong? afterId = default)
        => $"guilds/{guildId}/scheduled-events/{eventId}/users{GetUrlEncodedQueryParams(("limit", limit), ("with_member", withMember), ("before", beforeId), ("after", afterId))}";

    #endregion

    #region Guild Template

    public static string GuildTemplate(string templateCode)
        => $"guilds/templates/{templateCode}";

    public static string GuildTemplates(ulong guildId)
        => $"guilds/{guildId}/templates";

    public static string UpdateGuildTemplate(ulong guildId, string templateCode)
        => $"guilds/{guildId}/templates/{templateCode}";

    #endregion

    #region Invite

    public static string GetInvite(string code, bool? withCounts = default, bool? withExpiration = default, ulong? eventId = default)
        => $"invites/{code}{GetUrlEncodedQueryParams(("with_counts", withCounts), ("with_expiration", withExpiration), ("guild_scheduled_event_id", eventId))}";

    public static string DeleteInvite(string code)
        => $"invites/{code}";

    #endregion

    #region Stage Instances

    public static string CreateStageInstance
        => $"stage-instances";

    public static string StageInstance(ulong instanceId)
        => $"stage-instances/{instanceId}";

    #endregion

    #region Sticker

    public static string GetSticker(ulong stickerId)
        => $"stickers/{stickerId}";

    public static string ListSticketPacks
        => $"sticker-packs";

    public static string GuildStickers(ulong guildId)
        => $"guilds/{guildId}/stickers";

    public static string GuildSticker(ulong guildId, ulong stickerId)
        => $"guilds/{guildId}/stickers/{stickerId}";

    #endregion

    #region User

    public static string GetUser(ulong userId)
        => $"users/{userId}";

    public static string CurrentUser
        => $"users/@me";

    public static string GetCurrentUserGuilds(ulong? before = default, ulong? after = default, int? limit = default, bool? withCounts = default)
        => $"users/@me/guilds{GetUrlEncodedQueryParams(("before", before),
            ("after", after),
            ("limit", limit),
            ("with_counts", withCounts))}";

    public static string GetCurrentUserGuildMember(ulong guildId)
        => $"users/@me/guilds/{guildId}/member";

    public static string LeaveGuild(ulong guildId)
        => $"users/@me/guilds/{guildId}";

    public static string CreateDm
        => "users/@me/channels";

    public static string GetUserConnections
        => "users/@me/connections";

    public static string ApplicationRoleConnection(ulong applicationId)
        => $"users/@me/applications/{applicationId}/role-connection";

    #endregion

    #region Voice

    public static string ListVoiceRegions
        => $"voice/regions";

    #endregion

    #region Webhook

    public static string ChannelWebhook(ulong channelId)
        => $"channels/{channelId}/webhooks";

    public static string GuildWebhooks(ulong guildId)
        => $"guilds/{guildId}/webhooks";

    public static string Webhook(ulong webhookId)
        => $"webhooks/{webhookId}";

    public static string Webhook(ulong webhookId, string webhookToken)
        => $"webhooks/{webhookId}/{webhookToken}";

    public static string ExecuteWebhook(ulong webhookId, string webhookToken, bool? wait = default, ulong? threadId = default)
        => $"webhooks/{webhookId}/{webhookToken}{GetUrlEncodedQueryParams(("wait", wait), ("thread_id", threadId))}";

    public static string ExecuteSlackWebhook(ulong webhookId, string webhookToken, bool? wait = default, ulong? threadId = default)
        => $"webhooks/{webhookId}/{webhookToken}/slack{GetUrlEncodedQueryParams(("wait", wait), ("thread_id", threadId))}";

    public static string ExecuteGithubWebhook(ulong webhookId, string webhookToken, bool? wait = default, ulong? threadId = default)
        => $"webhooks/{webhookId}/{webhookToken}/github{GetUrlEncodedQueryParams(("wait", wait), ("thread_id", threadId))}";

    public static string WebhookMessage(ulong webhookId, string webhookToken, ulong messageId, ulong? threadId = default)
        => $"webhooks/{webhookId}/{webhookToken}/messages/{messageId}{GetUrlEncodedQueryParams(("thread_id", threadId))}";

    #endregion
}
