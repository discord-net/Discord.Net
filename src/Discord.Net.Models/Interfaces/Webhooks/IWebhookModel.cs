namespace Discord.Models.Webhooks;

public interface IWebhookModel : IEntityModel<ulong>
{
    int Type { get; }
    ulong? GuildId { get; }
    ulong? ChannelId { get; }
    ulong? UserId { get; }
    string? Name { get; }
    string? AvatarHash { get; }
    string? Token { get; }
    ulong? ApplicationId { get; }
    ulong SourceGuildId { get; }
    ulong SourceChannelId { get; }
    string? Url { get; }
}
