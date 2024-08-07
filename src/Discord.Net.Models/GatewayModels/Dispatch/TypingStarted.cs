using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class TypingStarted : ITypingStartPayloadData
{
    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("guild_id")]
    public Optional<ulong> GuildId { get; set; }

    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }

    [JsonPropertyName("timestamp")]
    public int Timestamp { get; set; }

    [JsonPropertyName("member")]
    public Optional<GuildMember> Member { get; set; }

    IMemberModel? ITypingStartPayloadData.Member => ~Member;
    ulong? ITypingStartPayloadData.GuildId => GuildId.ToNullable();
}
