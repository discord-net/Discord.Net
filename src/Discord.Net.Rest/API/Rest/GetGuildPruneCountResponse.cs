#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    internal class GetGuildPruneCountResponse
    {
        [JsonProperty("pruned")]
        public int Pruned { get; set; }
    }
}
