using System.Text.Json.Serialization;

namespace Discord.API;

public class IntegrationAccount
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}
