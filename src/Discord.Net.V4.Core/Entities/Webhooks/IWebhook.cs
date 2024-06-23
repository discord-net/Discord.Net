namespace Discord;

public interface IWebhook : ISnowflakeEntity, IWebhookActor
{
    WebhookType Type { get; }
    ILoadableGuildActor? Guild { get; }
    ILoadableChannelActor? Channel { get; }
    IUserActor? User { get; }
    string? Name { get; }
    string? Avatar { get; }
    string? Token { get; }
    ulong? ApplicationId { get; }
    ILoadableGuildActor? SourceGuild { get; }
    string? SourceGuildIcon { get; }
    string? SourceGuildName { get; }
    ILoadableNewsChannelActor? SourceChannel { get; }
    string? SourceChannelName { get; }
    string? Url { get; }
}
