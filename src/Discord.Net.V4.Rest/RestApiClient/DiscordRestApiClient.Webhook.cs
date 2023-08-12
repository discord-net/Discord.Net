using Discord.Models.RestRequests;
using Discord.Models.Webhooks;

namespace Discord.Rest;

public partial class DiscordRestApiClient
{
    public virtual Task<IWebhookModel> CreateWebhookAsync(string name, Image? avatar = null, CancellationToken? cancellationToken = null,
        RequestOptions? options = null) => throw new NotImplementedException();

    public virtual Task<IWebhookModel[]> GetChannelWebhooksAsync(ulong channelId, CancellationToken? cancellationToken = null, RequestOptions? options = null)
        => throw new NotImplementedException();

    public virtual Task<IWebhookModel[]> GetGuildWebhooks(ulong guildId, CancellationToken? cancellationToken = null, RequestOptions? options = null)
        => throw new NotImplementedException();

    public virtual Task<IWebhookModel> GetWebhookAsync(ulong webhookId, CancellationToken? cancellationToken = null, RequestOptions? options = null)
        => throw new NotImplementedException();

    public virtual Task<IWebhookModel> GetWebhookWithTokenAsync(ulong webhookId, string webhookToken, CancellationToken? cancellationToken = null,
        RequestOptions? options = null) => throw new NotImplementedException();

    public virtual Task<IWebhookModel> ModifyWebhookAsync(ulong webhookId, IModifyWebhookParams args, CancellationToken? cancellationToken = null,
        RequestOptions? options = null)
        => throw new NotImplementedException();

    public virtual Task<IWebhookModel> ModifyWebhookWithTokenAsync(ulong webhookId, string webhookToken, IModifyWebhookParams args, CancellationToken? cancellationToken = null,
        RequestOptions? options = null)
        => throw new NotImplementedException();

    public virtual Task DeleteWebhookAsync(ulong webhookId, CancellationToken? cancellationToken = null, RequestOptions? options = null)
        => throw new NotImplementedException();

    public virtual Task DeleteWebhookWithTokenAsync(ulong webhookId, string webhookToken, CancellationToken? cancellationToken = null, RequestOptions? options = null)
        => throw new NotImplementedException();
}
