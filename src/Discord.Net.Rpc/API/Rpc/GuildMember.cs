#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    internal class GuildMember
    {
        [JsonProperty("user")]
        public User User { get; set; }
        [JsonProperty("status")]
        public UserStatus Status { get; set; }
        /*[JsonProperty("activity")]
        public object Activity { get; set; }*/
    }
}
