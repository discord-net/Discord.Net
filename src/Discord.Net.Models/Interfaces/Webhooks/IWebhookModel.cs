namespace Discord.Models;

public interface IWebhookModel : IEntityModel<ulong>
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
    ulong? SourceChannelId { get; }
    string? Url { get; }
}
