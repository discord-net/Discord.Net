using Discord.API.Converters;
using Newtonsoft.Json;

namespace Discord.API.Client
{
    public class MessageReference
    {
        [JsonProperty("id"), JsonConverter(typeof(LongStringConverter))]
        public ulong Id { get; set; }
        [JsonProperty("message_id"), JsonConverter(typeof(LongStringConverter))] //Only used in MESSAGE_ACK
        public ulong MessageId { get { return Id; } set { Id = value; } }
        [JsonProperty("channel_id"), JsonConverter(typeof(LongStringConverter))]
        public ulong ChannelId { get; set; }
    }
}
