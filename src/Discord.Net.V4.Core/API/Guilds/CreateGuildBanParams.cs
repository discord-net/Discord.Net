using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class CreateGuildBanParams
{
    [JsonPropertyName("delete_message_seconds")]
    public Optional<int> DeleteMessageSeconds { get; set; }
}
