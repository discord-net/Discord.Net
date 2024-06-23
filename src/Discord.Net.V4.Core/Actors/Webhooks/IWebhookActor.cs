using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public interface ILoadableWebhookActor :
    IWebhookActor,
    ILoadableEntity<ulong, IWebhook>;

public interface IWebhookActor :
    IModifiable<ulong, IWebhookActor, ModifyWebhookProperties, ModifyWebhookParams>,
    IDeletable<ulong, IWebhookActor>,
    IActor<ulong, IWebhook>
{
    static IApiRoute IDeletable<ulong, IWebhookActor>.DeleteRoute(IPathable path, ulong id)
        => Routes.DeleteWebhook(id);

    static IApiInRoute<ModifyWebhookParams>
        IModifiable<ulong, IWebhookActor, ModifyWebhookProperties, ModifyWebhookParams>.ModifyRoute(IPathable path,
            ulong id, ModifyWebhookParams args)
        => Routes.ModifyWebhook(id, args);

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
