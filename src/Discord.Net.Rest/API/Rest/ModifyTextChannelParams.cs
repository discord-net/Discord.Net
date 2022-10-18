using System.Text.Json.Serialization;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ModifyTextChannelParams : ModifyGuildChannelParams
    {
        [JsonPropertyName("topic")]
        public Optional<string> Topic { get; set; }
        [JsonPropertyName("nsfw")]
        public Optional<bool> IsNsfw { get; set; }
        [JsonPropertyName("rate_limit_per_user")]
        public Optional<int> SlowModeInterval { get; set; }
    }
}
