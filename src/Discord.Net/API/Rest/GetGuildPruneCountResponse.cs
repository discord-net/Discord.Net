using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class GetGuildPruneCountResponse
    {
        [JsonProperty("pruned")]
        public int Pruned { get; set; }
    }
}
