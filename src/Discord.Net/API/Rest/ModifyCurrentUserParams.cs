using Newtonsoft.Json;
using System.IO;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ModifyCurrentUserParams
    {
        [JsonProperty("username")]
        internal Optional<string> _username { get; set; }
        public string Username { set { _username = value; } }

        [JsonProperty("avatar")]
        internal Optional<Image> _avatar { get; set; }
        public Stream Avatar { set { _avatar = new Image(value); } }
    }
}
