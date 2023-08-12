using System.Text.Json.Serialization;

namespace Discord.API.Rest;

internal class CreateGuildBanParams
{
    [JsonPropertyName("delete_message_seconds")]
    public Optional<int> DeleteMessageSeconds { get; set; }
}
