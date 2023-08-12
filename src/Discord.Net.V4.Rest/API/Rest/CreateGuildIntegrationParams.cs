using System.Text.Json.Serialization;

namespace Discord.API.Rest;

internal class CreateGuildIntegrationParams
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }
}
