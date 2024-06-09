using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class CreateGuildBanParams
{
    [JsonPropertyName("delete_message_seconds")]
    public Optional<int> DeleteMessageSeconds { get; set; }
}
