using Newtonsoft.Json;

namespace Discord.API.Rest
{
    internal class SessionStartLimit
    {
        [JsonProperty("total")]
        public int Total { get; set; }
        [JsonProperty("remaining")]
        public int Remaining { get; set; }
        [JsonProperty("reset_after")]
        public int ResetAfter { get; set; }
        [JsonProperty("max_concurrency")]
        public int MaxConcurrency { get; set; }
    }
}
