using Discord.API.Converters;
using Newtonsoft.Json;

namespace Discord.API.Client.GatewaySocket
{
    public class TypingStartEvent
    {
        [JsonProperty("user_id"), JsonConverter(typeof(LongStringConverter))]
        public ulong UserId { get; set; }
        [JsonProperty("channel_id"), JsonConverter(typeof(LongStringConverter))]
        public ulong ChannelId { get; set; }
        [JsonProperty("timestamp")]
        public int Timestamp { get; set; }
    }
}
