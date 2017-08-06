#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Gateway
{
    internal class HelloEvent
    {
        [ModelProperty("heartbeat_interval")]
        public int HeartbeatInterval { get; set; }
    }
}
