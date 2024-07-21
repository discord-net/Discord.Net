namespace Discord.Models;

[ModelEquality]
public partial interface IWebhookModel : IEntityModel<ulong>
{
    int Type { get; }
    ulong? GuildId { get; }
    ulong? ChannelId { get; }
    ulong? UserId { get; }
    string? Name { get; }
    string? Avatar { get; }
    string? Token { get; }
    ulong? ApplicationId { get; }
    ulong? SourceGuildId { get; }
    string? SourceGuildName { get; }
    string? SourceGuildIcon { get; }
    ulong? SourceChannelId { get; }
    string? SourceChannelName { get; }
    string? Url { get; }
}
