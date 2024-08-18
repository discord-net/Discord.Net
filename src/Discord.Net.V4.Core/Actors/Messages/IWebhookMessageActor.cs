using Discord.Models;
using Discord.Rest;

namespace Discord;

public partial interface IWebhookMessageActor :
    IMessageActor,
    IWebhookRelationship,
    IActor<ulong, IWebhookMessage>,
    IEntityProvider<IWebhookMessage, IMessageModel>
{
    [SourceOfTruth]
    internal new IWebhookMessage CreateEntity(IMessageModel model);
    
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

    //[return: TypeHeuristic(nameof(CreateEntity))]
    async Task<IWebhookMessage> ModifyAsync(
        string webhookToken,
        Action<ModifyWebhookMessageProperties> func,
        EntityOrId<ulong, IThreadChannel>? thread = null,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        var args = new ModifyWebhookMessageProperties();
        func(args);

        var message = await Client.RestApiClient.ExecuteRequiredAsync(
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

        return (IWebhookMessage)CreateEntity(message);
    }
}
