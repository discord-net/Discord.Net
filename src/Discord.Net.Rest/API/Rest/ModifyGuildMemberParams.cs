#pragma warning disable CS1591
using Newtonsoft.Json;

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
        public Optional<ulong> ChannelId { get; set; }
    }
}
