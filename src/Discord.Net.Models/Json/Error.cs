using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class Error
{
    [JsonPropertyName("code")]
    public required string Code { get; set; }

    [JsonPropertyName("message")]
    public required string Message { get; set; }
}
