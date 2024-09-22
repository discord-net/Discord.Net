using Discord.Models;
using Discord.Rest;

namespace Discord;

[FetchableOfMany(nameof(Routes.GetGuildWebhooks))]
public partial interface IChannelFollowerWebhook :
    IWebhook,
    IChannelFollowerWebhookActor,
    ISnowflakeEntity<IChannelFollowerWebhookModel>
{
    IGuildActor SourceGuild { get; }
    string SourceGuildName { get; }
    string? SourceGuildIcon { get; }
    INewsChannelActor SourceChannel { get; }
    string SourceChannelName { get; }

    [SourceOfTruth]
    new IChannelFollowerWebhookModel GetModel();
}
