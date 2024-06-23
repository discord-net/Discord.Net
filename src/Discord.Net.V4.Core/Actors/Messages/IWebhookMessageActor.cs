using Discord.Rest;

namespace Discord;

public interface ILoadableWebhookMessageActor :
    IWebhookMessageActor,
    ILoadableEntity<ulong, IWebhookMessage>;

public interface IWebhookMessageActor :
    IMessageActor
{
    Task DeleteAsync(
        string webhookToken,
        EntityOrId<ulong, IThreadChannel>? thread = null,
        RequestOptions? options = null,
        CancellationToken token = default
    ) => DeleteAsync(
        Client,
        Routes.DeleteWebhookMessage(Require<IWebhook>(), webhookToken, Id, thread?.Id),
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
                Require<IWebhook>(),
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
