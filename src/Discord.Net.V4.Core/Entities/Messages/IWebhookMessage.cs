using Discord.Rest;

namespace Discord;

public interface IWebhookMessage :
    IMessage,
    IWebhookMessageActor
{
    async Task RefreshAsync(
        string webhookToken,
        EntityOrId<ulong, IThreadChannel>? thread = null,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        var model = await Client.RestApiClient.ExecuteRequiredAsync(
            Routes.GetWebhookMessage(
                Webhook.Id,
                webhookToken,
                Id,
                thread?.Id
            ),
            options ?? Client.DefaultRequestOptions,
            token
        );

        await UpdateAsync(model, token);
    }

    new ILoadableWebhookActor Webhook { get; }

    ILoadableWebhookActor IMessage.Webhook => Webhook;
    ILoadableWebhookActor IWebhookRelationship.Webhook => Webhook;
}
