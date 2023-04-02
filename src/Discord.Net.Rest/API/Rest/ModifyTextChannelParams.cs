using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ModifyTextChannelParams : ModifyGuildChannelParams
    {
        [JsonProperty("topic")]
        public Optional<string> Topic { get; set; }

        [JsonProperty("nsfw")]
        public Optional<bool> IsNsfw { get; set; }

        [JsonProperty("rate_limit_per_user")]
        public Optional<int> SlowModeInterval { get; set; }

        [JsonProperty("default_thread_rate_limit_per_user")]
        public Optional<int> DefaultSlowModeInterval { get; set; }
    }
}
