using Newtonsoft.Json;

namespace Discord.API.Rest
{
    internal class GetInviteParams
    {
        [JsonProperty("with_counts")]
        public Optional<bool?> WithCounts { get; set; }
    }
}
