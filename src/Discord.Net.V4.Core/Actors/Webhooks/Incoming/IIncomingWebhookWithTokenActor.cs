using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

[
    Loadable(nameof(Routes.GetWebhookWithToken)),
    Modifiable<ModifyWebhookWithTokenProperties>(nameof(Routes.ModifyWebhookWithToken)),
    Deletable(nameof(Routes.DeleteWebhookWithToken))
]
public partial interface IIncomingWebhookWithTokenActor :
    IIncomingWebhookActor,
    ITokenPathProvider
{
    [SourceOfTruth] new ulong Id { get; }

    IWebhookMessageActor.Indexable Messages { get; }

    async Task<IWebhookMessage?> ExecuteAsync(
        ExecuteWebhookProperties args,
        bool? wait = null,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        var model = await Client.RestApiClient.ExecuteAsync(
            Routes.ExecuteWebhook(
                Id,
                Token,
                args.ToApiModel(),
                wait
            ),
            options,
            token
        );

        return model is null
            ? null
            : Messages.CreateEntity(model);
    }
}