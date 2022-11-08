using Newtonsoft.Json;

namespace Discord.API.Rest;


[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
internal class ModifyGuildWelcomeScreenParams
{
    [JsonProperty("enabled")]
    public Optional<bool> Enabled { get; set; }

    [JsonProperty("welcome_channels")]
    public Optional<WelcomeScreenChannel> Channels { get; set; }

    [JsonProperty("description")]
    public Optional<string> Description { get; set; }
}
