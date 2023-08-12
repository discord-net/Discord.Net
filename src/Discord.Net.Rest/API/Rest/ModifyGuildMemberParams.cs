using Newtonsoft.Json;
using System;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ModifyGuildMemberParams
    {
        [JsonProperty("mute")]
        public Optional<bool> Mute { get; set; }
        [JsonProperty("deaf")]
        public Optional<bool> Deaf { get; set; }
        [JsonProperty("nick")]
        public Optional<string> Nickname { get; set; }
        [JsonProperty("roles")]
        public Optional<ulong[]> RoleIds { get; set; }
        [JsonProperty("channel_id")]
        public Optional<ulong?> ChannelId { get; set; }
        [JsonProperty("communication_disabled_until")]
        public Optional<DateTimeOffset?> TimedOutUntil { get; set; }

        [JsonProperty("flags")]
        public Optional<GuildUserFlags> Flags { get; set; }
    }
}
