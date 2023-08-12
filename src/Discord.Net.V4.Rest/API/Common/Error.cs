using System.Text.Json.Serialization;

namespace Discord.API;

internal class Error
{
    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }
}
