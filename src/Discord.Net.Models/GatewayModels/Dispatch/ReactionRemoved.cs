using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ReactionRemoved : IMessageReactionRemovedPayloadData
{
    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }

    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("guild_id")]
    public Optional<ulong> GuildId { get; set; }

    [JsonPropertyName("emoji")]
    public required PartialEmote Emoji { get; set; }

    [JsonPropertyName("burst")]
    public bool IsBurst { get; set; }

    [JsonPropertyName("type")]
    public int Type { get; set; }

    IPartialEmoteModel IMessageReactionRemovedPayloadData.Emoji => Emoji;
    ulong? IMessageReactionRemovedPayloadData.GuildId => GuildId.ToNullable();
}
