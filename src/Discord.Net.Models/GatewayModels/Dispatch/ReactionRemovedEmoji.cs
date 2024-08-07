using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ReactionRemovedEmoji : IMessageReactionRemovedEmojiPayloadData
{
    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("guild_id")]
    public Optional<ulong> GuildId { get; set; }

    [JsonPropertyName("message_id")]
    public ulong MessageId { get; set; }

    [JsonPropertyName("emoji")]
    public required IEmoteModel Emoji { get; set; }

    ulong? IMessageReactionRemovedEmojiPayloadData.GuildId => GuildId.ToNullable();
}
