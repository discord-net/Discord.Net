using Newtonsoft.Json;

namespace Discord.API.Rest
{
    internal class SessionStartLimit
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }
        [JsonPropertyName("remaining")]
        public int Remaining { get; set; }
        [JsonPropertyName("reset_after")]
        public int ResetAfter { get; set; }
        [JsonPropertyName("max_concurrency")]
        public int MaxConcurrency { get; set; }
    }
}
