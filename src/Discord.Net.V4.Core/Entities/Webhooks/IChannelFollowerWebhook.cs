using Discord.Models;
using Discord.Rest;

namespace Discord;

[FetchableOfMany(nameof(Routes.GetGuildWebhooks))]
public partial interface IChannelFollowerWebhook :
    IWebhook,
    IChannelFollowerWebhookActor
{
    IGuildActor? SourceGuild { get; }
    string? SourceGuildIcon { get; }
    string? SourceGuildName { get; }
    INewsChannelActor? SourceChannel { get; }
    string? SourceChannelName { get; }
}
