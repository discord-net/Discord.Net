using Discord.API.Converters;
using Newtonsoft.Json;

namespace Discord.API.Client.GatewaySocket
{
    public sealed class TypingStartEvent
    {
        [JsonProperty("user_id"), JsonConverter(typeof(LongStringConverter))]
        public ulong UserId { get; }
        [JsonProperty("channel_id"), JsonConverter(typeof(LongStringConverter))]
        public ulong ChannelId { get; }
        [JsonProperty("timestamp")]
        public int Timestamp { get; }
    }
}
