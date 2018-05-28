#pragma warning disable CS1591
using Newtonsoft.Json;
using System;

namespace Discord.API.Voice
{
    internal class ReadyEvent
    {
        [JsonProperty("ssrc")]
        public uint SSRC { get; set; }
        [JsonProperty("ip")]
        public string Ip { get; set; }
        [JsonProperty("port")]
        public ushort Port { get; set; }
        [JsonProperty("modes")]
        public string[] Modes { get; set; }
        [JsonProperty("heartbeat_interval")]
        [Obsolete("This field is errorneous and should not be used", true)]
        public int HeartbeatInterval { get; set; }
    }
}
