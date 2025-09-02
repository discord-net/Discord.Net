using Newtonsoft.Json;

namespace Discord.API.Voice
{
    internal class HeartbeatParams
    {
        [JsonProperty("t")]
        public long Timestamp { get; set; }
        [JsonProperty("seq_ack")]
        public int SequenceAck { get; set; }
    }
}
