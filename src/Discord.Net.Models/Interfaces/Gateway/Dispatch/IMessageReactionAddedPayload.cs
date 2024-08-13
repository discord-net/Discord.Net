namespace Discord.Models;

public interface IMessageReactionAddedPayload : IGatewayPayloadData
{
    ulong UserId { get; }
    ulong ChannelId { get; }
    ulong MessageId { get; }
    ulong? GuildId { get; }
    IMemberModel? Member { get; }
    IPartialEmoteModel Emoji { get; }
    ulong? MessageAuthorId { get; }
    bool IsBurst { get; }
    string[]? BurstColors { get; }
    int Type { get; }
}
