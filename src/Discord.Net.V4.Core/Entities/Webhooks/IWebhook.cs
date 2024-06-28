using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

using IModifiable = IModifiable<ulong, IWebhook, ModifyWebhookProperties, ModifyWebhookParams, IWebhookModel>;

public interface IWebhook :
    ISnowflakeEntity,
    IWebhookActor,
    IModifiable,
    IRefreshable<IWebhook, ulong, IWebhookModel>
{
    static IApiOutRoute<IWebhookModel> IRefreshable<IWebhook, ulong, IWebhookModel>.RefreshRoute(
        IWebhook self,
        ulong id
    ) => Routes.GetWebhook(id);

    static IApiInOutRoute<ModifyWebhookParams, IEntityModel> IModifiable.ModifyRoute(
        IPathable path,
        ulong id,
        ModifyWebhookParams args
    ) => Routes.ModifyWebhook(id, args);

    async Task RefreshWithTokenAsync(string webhookToken, RequestOptions? options = null,
        CancellationToken token = default)
    {
        var model = await Client.RestApiClient.ExecuteRequiredAsync(
            Routes.GetWebhook(Id),
            options ?? Client.DefaultRequestOptions,
            token
        );

        await UpdateAsync(model, token);
    }

    WebhookType Type { get; }
    ILoadableGuildActor? Guild { get; }
    ILoadableChannelActor? Channel { get; }
    IUserActor? User { get; }
    string? Name { get; }
    string? Avatar { get; }
    string? Token { get; }
    ulong? ApplicationId { get; }
    ILoadableGuildActor? SourceGuild { get; }
    string? SourceGuildIcon { get; }
    string? SourceGuildName { get; }
    ILoadableNewsChannelActor? SourceChannel { get; }
    string? SourceChannelName { get; }
    string? Url { get; }
}
