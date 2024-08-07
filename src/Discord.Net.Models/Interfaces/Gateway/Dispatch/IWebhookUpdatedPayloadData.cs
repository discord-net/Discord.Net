namespace Discord.Models;

public interface IWebhookUpdatedPayloadData : IGatewayPayloadData
{
    ulong GuildId { get; }
    ulong ChannelId { get; }
}
