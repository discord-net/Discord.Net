using Newtonsoft.Json;

namespace Discord.API;

internal class ReactionCountDetails
{
    [JsonProperty("normal")]
    public int NormalCount { get; set;}

    [JsonProperty("burst")]
    public int BurstCount { get; set;}
}
