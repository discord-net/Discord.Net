#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Voice
{
    internal class ReadyEvent
    {
        [ModelProperty("ssrc")]
        public uint SSRC { get; set; }
        [ModelProperty("ip")]
        public string Ip { get; set; }
        [ModelProperty("port")]
        public ushort Port { get; set; }
        [ModelProperty("modes")]
        public string[] Modes { get; set; }
        [ModelProperty("heartbeat_interval")]
        public int HeartbeatInterval { get; set; }
    }
}
