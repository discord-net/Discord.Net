using Discord.Models.Json;
using Discord.Utils;

namespace Discord.Rest;

public partial class Routes
{
    public static ApiBodyRoute<CreateWebhookParams, Webhook> CreateChannelWebhook(ulong channelId,
        CreateWebhookParams body)
        => new(nameof(CreateChannelWebhook),
            RequestMethod.Post,
            $"channels/{channelId}/webhooks",
            body,
            ContentType.JsonBody,
            (ScopeType.Channel, channelId));

    public static ApiRoute<Webhook[]> GetChannelWebhooks(ulong channelId)
        => new(nameof(GetChannelWebhooks),
            RequestMethod.Get,
            $"channels/{channelId}/webhooks",
            (ScopeType.Channel, channelId));

    public static ApiRoute<Webhook[]> GetGuildWebhooks(ulong guildId)
        => new(nameof(GetGuildWebhooks),
            RequestMethod.Get,
            $"guilds/{guildId}/webhooks",
            (ScopeType.Guild, guildId));

    public static ApiRoute<Webhook> GetWebhook(ulong webhookId)
        => new(nameof(GetWebhook),
            RequestMethod.Get,
            $"webhooks/{webhookId}");

    public static ApiRoute<Webhook> GetWebhookWithToken(ulong webhookId, string token)
        => new(nameof(GetWebhookWithToken),
            RequestMethod.Get,
            $"webhooks/{webhookId}/{token}");

    public static ApiBodyRoute<ModifyWebhookParams, Webhook> ModifyWebhook(ulong webhookId, ModifyWebhookParams body)
        => new(nameof(ModifyWebhook),
            RequestMethod.Patch,
            $"webhooks/{webhookId}",
            body);

    public static ApiBodyRoute<ModifyWebhookWithTokenParams, Webhook> ModifyWebhookWithToken(ulong webhookId,
        string token, ModifyWebhookWithTokenParams body)
        => new(nameof(ModifyWebhookWithToken),
            RequestMethod.Patch,
            $"webhooks/{webhookId}/{token}",
            body);

    public static BasicApiRoute DeleteWebhook(ulong webhookId)
        => new(nameof(DeleteWebhook),
            RequestMethod.Delete,
            $"webhooks/{webhookId}");

    public static BasicApiRoute DeleteWebhookWithToken(ulong webhookId, string token)
        => new(nameof(DeleteWebhookWithToken),
            RequestMethod.Delete,
            $"webhooks/{webhookId}/{token}");

    // TODO: Add support for multipart/form-data
    public static ApiBodyRoute<ExecuteWebhookParams, Message> ExecuteWebhook(ulong webhookId, string token,
        ExecuteWebhookParams body, bool wait = false, ulong? threadId = default)
        => new(nameof(ExecuteWebhook),
            RequestMethod.Post,
            $"webhooks/{webhookId}/{token}{RouteUtils.GetUrlEncodedQueryParams(("wait", wait), ("thread_id", threadId))}",
            body,
            ContentType.JsonBody,
            (ScopeType.Webhook, webhookId));

    public static ApiRoute<Message> GetWebhookMessage(ulong webhookId, string token, ulong messageId,
        ulong? threadId = default)
        => new(nameof(GetWebhookMessage),
            RequestMethod.Get,
            $"webhooks/{webhookId}/{token}/messages/{messageId}{RouteUtils.GetUrlEncodedQueryParams(("thread_id", threadId))}",
            (ScopeType.Webhook, webhookId));


    // TODO: Add support for multipart/form-data
    public static ApiBodyRoute<ModifyWebhookMessageParams, Message> ModifyWebhookMessage(ulong webhookId, string token,
        ulong messageId, ModifyWebhookMessageParams body, ulong? threadId = default)
        => new(nameof(ModifyWebhookMessage),
            RequestMethod.Patch,
            $"webhooks/{webhookId}/{token}/messages/{messageId}{RouteUtils.GetUrlEncodedQueryParams(("thread_id", threadId))}",
            body,
            ContentType.JsonBody,
            (ScopeType.Webhook, webhookId));

    public static BasicApiRoute DeleteWebhookMessage(ulong webhookId, string token, ulong messageId, ulong? threadId)
        => new(nameof(DeleteWebhookMessage),
            RequestMethod.Delete,
            $"webhooks/{webhookId}/{token}/messages/{messageId}{RouteUtils.GetUrlEncodedQueryParams(("thread_id", threadId))}",
            (ScopeType.Webhook, webhookId));
}
