#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rpc
{
    internal class GuildStatusEvent
    {
        [ModelProperty("guild")]
        public Guild Guild { get; set; }
        [ModelProperty("online")]
        public int Online { get; set; }
    }
}
