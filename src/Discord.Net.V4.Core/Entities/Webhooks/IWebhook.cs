using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

[FetchableOfMany(nameof(Routes.GetGuildWebhooks))]
[FetchableOfMany(nameof(Routes.GetChannelWebhooks))]
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
    IUserActor? Creator { get; }
    string? Name { get; }
    string? Avatar { get; }
    ulong? ApplicationId { get; }
}
