using Newtonsoft.Json;

namespace Discord.API;

internal class WelcomeScreen
{
    [JsonProperty("description")]
    public Optional<string> Description { get; set; }

    [JsonProperty("welcome_channels")]
    public Optional<WelcomeScreenChannel[]> WelcomeChannels { get; set; }
}
