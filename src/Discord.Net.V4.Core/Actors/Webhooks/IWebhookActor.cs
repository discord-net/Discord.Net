using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

[Loadable(nameof(Routes.GetWebhook))]
[Modifiable<ModifyWebhookProperties>(nameof(Routes.ModifyWebhook))]
[Deletable(nameof(Routes.DeleteWebhook))]
public partial interface IWebhookActor :
    IActor<ulong, IWebhook>
{
    async Task<IMessage?> GetWebhookMessageAsync(
        string webhookToken,
        ulong messageId,
        EntityOrId<ulong, IThreadChannel>? thread = null,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        var model = await Client.RestApiClient.ExecuteAsync(
            Routes.GetWebhookMessage(Id, webhookToken, messageId, thread?.Id),
            options ?? Client.DefaultRequestOptions,
            token
        );

        return Client.CreateNullableEntity(model);
    }

    Task ModifyWithTokenAsync(
        string webhookToken,
        Action<ModifyWebhookWithTokenProperties> func,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        var args = new ModifyWebhookWithTokenProperties();
        func(args);
        return Client.RestApiClient.ExecuteAsync(
            Routes.ModifyWebhookWithToken(Id, webhookToken, args.ToApiModel()),
            options ?? Client.DefaultRequestOptions,
            token
        );
    }

    Task DeleteWithTokenAsync(
        string webhookToken,
        RequestOptions? options = null,
        CancellationToken token = default
    ) => DeleteAsync(Client, Routes.DeleteWebhookWithToken(Id, webhookToken), options, token);
}
