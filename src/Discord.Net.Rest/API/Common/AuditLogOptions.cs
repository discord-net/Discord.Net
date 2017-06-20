using Newtonsoft.Json;

namespace Discord.API
{
    // TODO: Complete this with all possible values for options
    internal class AuditLogOptions
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("channel_id")]
        public ulong ChannelId { get; set; }
    }
}
