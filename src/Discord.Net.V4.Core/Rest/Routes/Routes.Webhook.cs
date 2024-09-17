using Discord.Models.Json;
using Discord.Utils;

namespace Discord.Rest;

public partial class Routes
{
    public static IApiInOutRoute<CreateWebhookParams, Webhook> CreateChannelWebhook(
        [IdHeuristic<IIntegrationChannel>] ulong channelId,
        CreateWebhookParams body) =>
        new ApiInOutRoute<CreateWebhookParams, Webhook>(nameof(CreateChannelWebhook), RequestMethod.Post,
            $"channels/{channelId}/webhooks", body, ContentType.JsonBody, (ScopeType.Channel, channelId));

    public static IApiOutRoute<Webhook[]> GetChannelWebhooks([IdHeuristic<IIntegrationChannel>] ulong channelId) =>
        new ApiOutRoute<Webhook[]>(nameof(GetChannelWebhooks), RequestMethod.Get, $"channels/{channelId}/webhooks",
            (ScopeType.Channel, channelId));

    public static IApiOutRoute<Webhook[]> GetGuildWebhooks([IdHeuristic<IGuild>] ulong guildId) =>
        new ApiOutRoute<Webhook[]>(nameof(GetGuildWebhooks), RequestMethod.Get, $"guilds/{guildId}/webhooks",
            (ScopeType.Guild, guildId));

    public static IApiOutRoute<Webhook> GetWebhook([IdHeuristic<IWebhook>] ulong webhookId) =>
        new ApiOutRoute<Webhook>(nameof(GetWebhook), RequestMethod.Get, $"webhooks/{webhookId}");

    public static IApiOutRoute<Webhook> GetWebhookWithToken(
        [IdHeuristic<IWebhook>] ulong webhookId,
        [IdHeuristic<ITokenPathProvider>] string token
        ) =>
        new ApiOutRoute<Webhook>(nameof(GetWebhookWithToken), RequestMethod.Get, $"webhooks/{webhookId}/{token}");

    public static IApiInOutRoute<ModifyWebhookParams, Webhook>
        ModifyWebhook([IdHeuristic<IWebhook>] ulong webhookId, ModifyWebhookParams body) =>
        new ApiInOutRoute<ModifyWebhookParams, Webhook>(nameof(ModifyWebhook), RequestMethod.Patch,
            $"webhooks/{webhookId}", body);

    public static IApiInOutRoute<ModifyWebhookWithTokenParams, Webhook> ModifyWebhookWithToken(
        [IdHeuristic<IWebhook>] ulong webhookId,
        [IdHeuristic<ITokenPathProvider>]
        string token,
        ModifyWebhookWithTokenParams body) =>
        new ApiInOutRoute<ModifyWebhookWithTokenParams, Webhook>(nameof(ModifyWebhookWithToken), RequestMethod.Patch,
            $"webhooks/{webhookId}/{token}", body);

    public static IApiRoute DeleteWebhook([IdHeuristic<IWebhook>] ulong webhookId) =>
        new ApiRoute(nameof(DeleteWebhook), RequestMethod.Delete, $"webhooks/{webhookId}");

    public static IApiRoute DeleteWebhookWithToken(
        [IdHeuristic<IWebhook>] ulong webhookId,
        [IdHeuristic<ITokenPathProvider>]
        string token) =>
        new ApiRoute(nameof(DeleteWebhookWithToken), RequestMethod.Delete, $"webhooks/{webhookId}/{token}");

    // TODO: Add support for multipart/form-data
    public static IApiInOutRoute<ExecuteWebhookParams, Message> ExecuteWebhook(
        [IdHeuristic<IWebhook>] ulong webhookId,
        [IdHeuristic<ITokenPathProvider>]
        string token,
        ExecuteWebhookParams body,
        bool? wait = null,
        ulong? threadId = default
    ) => new ApiInOutRoute<ExecuteWebhookParams, Message>(
        nameof(ExecuteWebhook),
        RequestMethod.Post,
        $"webhooks/{webhookId}/{token}{RouteUtils.GetUrlEncodedQueryParams(("wait", wait), ("thread_id", threadId))}",
        body, ContentType.JsonBody, (ScopeType.Webhook, webhookId)
    );

    public static IApiOutRoute<Message> GetWebhookMessage(
        [IdHeuristic<IWebhook>] ulong webhookId,
        [IdHeuristic<ITokenPathProvider>]
        string token,
        [IdHeuristic<IWebhookMessage>] ulong messageId,
        ulong? threadId = default) =>
        new ApiOutRoute<Message>(nameof(GetWebhookMessage), RequestMethod.Get,
            $"webhooks/{webhookId}/{token}/messages/{messageId}{RouteUtils.GetUrlEncodedQueryParams(("thread_id", threadId))}",
            (ScopeType.Webhook, webhookId));


    // TODO: Add support for multipart/form-data
    public static IApiInOutRoute<ModifyWebhookMessageParams, Message> ModifyWebhookMessage(
        [IdHeuristic<IWebhook>] ulong webhookId,
        [IdHeuristic<ITokenPathProvider>]
        string token,
        [IdHeuristic<IWebhookMessage>] ulong messageId, ModifyWebhookMessageParams body,
        [IdHeuristic<IThreadChannel>] ulong? threadId = default
    ) =>
        new ApiInOutRoute<ModifyWebhookMessageParams, Message>(nameof(ModifyWebhookMessage), RequestMethod.Patch,
            $"webhooks/{webhookId}/{token}/messages/{messageId}{RouteUtils.GetUrlEncodedQueryParams(("thread_id", threadId))}",
            body, ContentType.JsonBody, (ScopeType.Webhook, webhookId));

    public static IApiRoute DeleteWebhookMessage(
        [IdHeuristic<IWebhook>] ulong webhookId,
        [IdHeuristic<ITokenPathProvider>]
        string token,
        [IdHeuristic<IWebhookMessage>] ulong messageId,
        [IdHeuristic<IThreadChannel>] ulong? threadId
    ) =>
        new ApiRoute(nameof(DeleteWebhookMessage), RequestMethod.Delete,
            $"webhooks/{webhookId}/{token}/messages/{messageId}{RouteUtils.GetUrlEncodedQueryParams(("thread_id", threadId))}",
            (ScopeType.Webhook, webhookId));
}