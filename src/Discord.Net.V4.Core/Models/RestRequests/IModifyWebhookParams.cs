namespace Discord.Models.RestRequests;

public interface IModifyWebhookParams
{
    Optional<string> Name { get; }
    Optional<Image> Avatar { get; }
    Optional<ulong> ChannelId { get; }
}
