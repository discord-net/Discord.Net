#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    internal class GuildStatusEvent
    {
        [JsonProperty("guild")]
        public Guild Guild { get; set; }
        [JsonProperty("online")]
        public int Online { get; set; }
    }
}
