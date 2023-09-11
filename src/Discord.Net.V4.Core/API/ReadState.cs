using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class ReadState
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("mention_count")]
    public int MentionCount { get; set; }

    [JsonPropertyName("last_message_id")]
    public Optional<ulong> LastMessageId { get; set; }
}
