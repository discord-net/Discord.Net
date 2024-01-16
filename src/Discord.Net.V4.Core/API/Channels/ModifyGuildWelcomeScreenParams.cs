using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class ModifyGuildWelcomeScreenParams
{
    [JsonPropertyName("enabled")]
    public Optional<bool> IsEnabled { get; set; }

    [JsonPropertyName("welcome_channels")]
    public Optional<WelcomeScreenChannel[]> WelcomeChannels { get; set; }

    [JsonPropertyName("description")]
    public Optional<string> Description { get; set; }
}
