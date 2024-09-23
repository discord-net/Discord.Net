using Discord.Models;
using Discord.Rest;

namespace Discord;

//[FetchableOfMany(nameof(Routes.GetGuildWebhooks))]
public partial interface IIncomingWebhook :
    IWebhook,
    IIncomingWebhookWithTokenActor,
    ISnowflakeEntity<IIncomingWebhookModel>
{
    string? Url { get; }

    [SourceOfTruth]
    new IIncomingWebhookModel GetModel();
}
