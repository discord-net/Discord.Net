namespace Discord.Models;

[ModelEquality]
public partial interface IChannelFollowerWebhookModel : IWebhookModel
{
    ulong GuildId { get; }
    ulong ChannelId { get; }
    ulong? SourceGuildId { get; }
    string? SourceGuildName { get; }
    string? SourceGuildIcon { get; }
    ulong? SourceChannelId { get; }
    string? SourceChannelName { get; }
}