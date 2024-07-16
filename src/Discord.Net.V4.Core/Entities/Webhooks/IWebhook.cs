using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

[Refreshable(nameof(Routes.GetWebhook))]
public partial interface IWebhook :
    ISnowflakeEntity<IWebhookModel>,
    IWebhookActor
{
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
    IGuildActor? Guild { get; }
    IChannelActor? Channel { get; }
    IUserActor? User { get; }
    string? Name { get; }
    string? Avatar { get; }
    string? Token { get; }
    ulong? ApplicationId { get; }
    IGuildActor? SourceGuild { get; }
    string? SourceGuildIcon { get; }
    string? SourceGuildName { get; }
    INewsChannelActor? SourceChannel { get; }
    string? SourceChannelName { get; }
    string? Url { get; }
}
