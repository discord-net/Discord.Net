using System.Text.Json.Serialization;

namespace Discord.API.Rest;

internal class ModifyGuildWelcomeScreenParams
{
    [JsonPropertyName("enabled")]
    public Optional<bool> Enabled { get; set; }

    [JsonPropertyName("welcome_channels")]
    public Optional<WelcomeScreenChannel[]> WelcomeChannels { get; set; }

    [JsonPropertyName("description")]
    public Optional<string> Description { get; set; }
}
