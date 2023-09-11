using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class Error
{
    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }
}
