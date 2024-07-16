using Discord.Rest;

namespace Discord;

public interface IWebhookMessageActor :
    IMessageActor,
    IWebhookRelationship,
    IActor<ulong, IWebhookMessage>
{
    Task DeleteAsync(
        string webhookToken,
        EntityOrId<ulong, IThreadChannel>? thread = null,
        RequestOptions? options = null,
        CancellationToken token = default
    ) => DeleteAsync(
        Client,
        Routes.DeleteWebhookMessage(Webhook.Id, webhookToken, Id, thread?.Id),
        options,
        token
    );

    Task ModifyAsync(
        string webhookToken,
        Action<ModifyWebhookMessageProperties> func,
        EntityOrId<ulong, IThreadChannel>? thread = null,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        var args = new ModifyWebhookMessageProperties();
        func(args);
        return Client.RestApiClient.ExecuteAsync(
            Routes.ModifyWebhookMessage(
                Webhook.Id,
                webhookToken,
                Id,
                args.ToApiModel(),
                thread?.Id
            ),
            options ?? Client.DefaultRequestOptions,
            token
        );
    }
}
