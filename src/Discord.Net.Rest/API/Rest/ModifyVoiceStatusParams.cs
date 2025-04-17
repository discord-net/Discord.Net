using Newtonsoft.Json;

namespace Discord.API.Rest;

internal class ModifyVoiceStatusParams
{
    [JsonProperty("status")]
    public string Status { get; set; }
}
