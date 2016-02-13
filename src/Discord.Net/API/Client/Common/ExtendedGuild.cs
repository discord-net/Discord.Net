using Newtonsoft.Json;

namespace Discord.API.Client
{
    public class ExtendedGuild : Guild
    {
        [JsonProperty("member_count")]
        public int? MemberCount { get; set; }
        [JsonProperty("large")]
        public bool IsLarge { get; set; }
        [JsonProperty("unavailable")]
        public bool? Unavailable { get; set; }

        [JsonProperty("channels")]
        public Channel[] Channels { get; set; }
        [JsonProperty("members")]
        public ExtendedMember[] Members { get; set; }
        [JsonProperty("presences")]
        public MemberPresence[] Presences { get; set; }
        [JsonProperty("voice_states")]
        public MemberVoiceState[] VoiceStates { get; set; }
    }
}
