using Discord.Rest;
using System.Diagnostics.CodeAnalysis;
using Discord.Models;

namespace Discord;

[Loadable(nameof(Routes.GetWebhook))]
[Modifiable<ModifyWebhookProperties>(nameof(Routes.ModifyWebhook))]
[SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
public partial interface IIncomingWebhookActor :
    IGuildChannelWebhookActor,
    IActor<ulong, IIncomingWebhook>,
    IEntityProvider<IWebhookMessage, IMessageModel>
{
    async Task<IWebhookMessage?> GetWebhookMessageAsync(
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

        return model is null ? null : CreateEntity(model);
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
