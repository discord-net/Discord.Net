using Discord.Rest;

namespace Discord;

[FetchableOfMany(nameof(Routes.GetGuildWebhooks))]
public partial interface IIncomingWebhook :
    IWebhook,
    IIncomingWebhookWithTokenActor
{
    string? Url { get; }
}
