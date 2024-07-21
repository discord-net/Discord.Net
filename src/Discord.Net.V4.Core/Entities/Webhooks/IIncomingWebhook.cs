using Discord.Rest;

namespace Discord;

[FetchableOfMany(nameof(Routes.GetGuildWebhooks))]
public partial interface IIncomingWebhook :
    IWebhook,
    IIncomingWebhookActor
{
    string? Token { get; }
    string? Url { get; }
}
