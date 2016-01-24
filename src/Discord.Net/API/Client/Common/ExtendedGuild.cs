using Newtonsoft.Json;

namespace Discord.API.Client
{
    public class ExtendedGuild : Guild
    {
        public class ExtendedMemberInfo : Member
        {
            [JsonProperty("mute")]
            public bool? IsServerMuted { get; set; }
            [JsonProperty("deaf")]
            public bool? IsServerDeafened { get; set; }
        }

        [JsonProperty("channels")]
        public Channel[] Channels { get; set; }
        [JsonProperty("members")]
        public ExtendedMemberInfo[] Members { get; set; }
        [JsonProperty("presences")]
        public MemberPresence[] Presences { get; set; }
        [JsonProperty("voice_states")]
        public MemberVoiceState[] VoiceStates { get; set; }
        [JsonProperty("large")]
        public bool IsLarge { get; set; }
        [JsonProperty("unavailable")]
        public bool? Unavailable { get; set; }
    }
}
