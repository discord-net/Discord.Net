namespace Discord.Models.Webhooks;

public interface IWebhookModel : IEntityModel<ulong>
{
    WebhookType Type { get; }
    ulong? GuildId { get; }
    ulong? ChannelId { get; }
    IUserModel? User { get; }
    string? Name { get; }
    string? AvatarHash { get; }
    string? Token { get; }
    ulong? ApplicationId { get; }
    IPartialGuildModel SourceGuild { get; }
    IChannelModel SourceChannel { get; }
    string? Url { get; }
}
