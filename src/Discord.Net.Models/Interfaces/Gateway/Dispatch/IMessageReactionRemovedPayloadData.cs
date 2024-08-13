namespace Discord.Models;

public interface IMessageReactionRemovedPayloadData : IGatewayPayloadData
{
    ulong UserId { get; }
    ulong ChannelId { get; }
    ulong? GuildId { get; }
    IPartialEmoteModel Emoji { get; }
    bool IsBurst { get; }
    int Type { get; }
}
