using Newtonsoft.Json;

namespace Discord.API.Rest;

internal class ModifyCurrentMemberParams
{
    [JsonProperty("nick")]
    public Optional<string> Nickname { get; set; }

    [JsonProperty("banner")]
    public Optional<Image?> Banner { get; set; }

    [JsonProperty("avatar")]
    public Optional<Image?> Avatar { get; set; }

    [JsonProperty("bio")]
    public Optional<string> Bio { get; set; }
}
