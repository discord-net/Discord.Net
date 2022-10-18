using System.Text.Json.Serialization;
using System;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ModifyGuildMemberParams
    {
        [JsonPropertyName("mute")]
        public Optional<bool> Mute { get; set; }
        [JsonPropertyName("deaf")]
        public Optional<bool> Deaf { get; set; }
        [JsonPropertyName("nick")]
        public Optional<string> Nickname { get; set; }
        [JsonPropertyName("roles")]
        public Optional<ulong[]> RoleIds { get; set; }
        [JsonPropertyName("channel_id")]
        public Optional<ulong?> ChannelId { get; set; }
        [JsonPropertyName("communication_disabled_until")]
        public Optional<DateTimeOffset?> TimedOutUntil { get; set; }
    }
}
