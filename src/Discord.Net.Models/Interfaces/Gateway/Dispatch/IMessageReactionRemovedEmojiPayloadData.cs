namespace Discord.Models;

public interface IMessageReactionRemovedEmojiPayloadData : IGatewayPayloadData
{
    ulong ChannelId { get; }
    ulong? GuildId { get; }
    ulong MessageId { get; }
    IPartialEmoteModel Emoji { get; }
}
