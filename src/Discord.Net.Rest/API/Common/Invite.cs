#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API
{
    internal class Invite
    {
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("guild")]
        public InviteGuild Guild { get; set; }
        [JsonProperty("channel")]
        public InviteChannel Channel { get; set; }
    }
}
