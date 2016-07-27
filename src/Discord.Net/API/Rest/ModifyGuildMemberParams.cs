using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ModifyGuildMemberParams
    {
        [JsonProperty("mute")]
        internal Optional<bool> _mute;
        public bool Mute { set { _mute = value; } }

        [JsonProperty("deaf")]
        internal Optional<bool> _deaf;
        public bool Deaf { set { _deaf = value; } }

        [JsonProperty("nick")]
        internal Optional<string> _nickname;
        public string Nickname { set { _nickname = value; } }

        [JsonProperty("roles")]
        internal Optional<ulong[]> _roleIds;
        public IEnumerable<ulong> RoleIds { set { _roleIds = value.ToArray(); } }
        public IEnumerable<IRole> Roles { set { _roleIds = value.Select(x => x.Id).ToArray(); } }

        [JsonProperty("channel_id")]
        internal Optional<ulong> _channelId;
        public ulong VoiceChannelId { set { _channelId = value; } }
        public IVoiceChannel VoiceChannel { set { _channelId = value.Id; } }
    }
}
