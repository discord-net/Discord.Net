using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Discord.API.Rest
{
    public class ModifyGuildMemberParams
    {
        [JsonProperty("mute")]
        public Optional<bool> Mute { get; set; }
        [JsonProperty("deaf")]
        public Optional<bool> Deaf { get; set; }
        [JsonProperty("nick")]
        public Optional<string> Nickname { get; set; }

        [JsonProperty("roles")]
        public Optional<IEnumerable<ulong>> RoleIds { get; set; }
        [JsonIgnore]
        public Optional<IEnumerable<IRole>> Roles { set { RoleIds = value.IsSpecified ? Optional.Create(value.Value.Select(x => x.Id)) : Optional.Create<IEnumerable<ulong>>(); } }

        [JsonProperty("channel_id")]
        public Optional<ulong> VoiceChannelId { get; set; }
        [JsonIgnore]
        public Optional<IVoiceChannel> VoiceChannel { set { VoiceChannelId = value.IsSpecified ? value.Value.Id : Optional.Create<ulong>(); } }
    }
}
