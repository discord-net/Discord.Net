using Newtonsoft.Json;

namespace Discord.API.Client.VoiceSocket
{
    public class ReadyEvent
    {
        [JsonProperty("ssrc")]
        public uint SSRC { get; set; }
        [JsonProperty("port")]
        public ushort Port { get; set; }
        [JsonProperty("modes")]
        public string[] Modes { get; set; }
        [JsonProperty("heartbeat_interval")]
        public int HeartbeatInterval { get; set; }
    }
}
