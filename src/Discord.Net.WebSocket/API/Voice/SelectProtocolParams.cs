#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Voice
{
    internal class SelectProtocolParams
    {
        [ModelProperty("protocol")]
        public string Protocol { get; set; }
        [ModelProperty("data")]
        public UdpProtocolInfo Data { get; set; }
    }
}
