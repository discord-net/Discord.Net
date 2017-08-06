#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Voice
{
    internal class UdpProtocolInfo
    {
        [ModelProperty("address")]
        public string Address { get; set; }
        [ModelProperty("port")]
        public int Port { get; set; }
        [ModelProperty("mode")]
        public string Mode { get; set; }
    }
}
