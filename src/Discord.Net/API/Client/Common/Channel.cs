using Discord.API.Converters;
using Newtonsoft.Json;

namespace Discord.API.Client
{
    public class Channel : ChannelReference
    {
        public class PermissionOverwrite
        {
            [JsonProperty("type")]
            public string Type { get; set; }
            [JsonProperty("id"), JsonConverter(typeof(LongStringConverter))]
            public ulong Id { get; set; }
            [JsonProperty("deny")]
            public uint Deny { get; set; }
            [JsonProperty("allow")]
            public uint Allow { get; set; }
        }

        [JsonProperty("last_message_id"), JsonConverter(typeof(NullableLongStringConverter))]
        public ulong? LastMessageId { get; set; }
        [JsonProperty("is_private")]
        public bool? IsPrivate { get; set; }
        [JsonProperty("position")]
        public int? Position { get; set; }
        [JsonProperty("topic")]
        public string Topic { get; set; }
        [JsonProperty("permission_overwrites")]
        public PermissionOverwrite[] PermissionOverwrites { get; set; }
        [JsonProperty("recipient")]
        public UserReference Recipient { get; set; }
    }
}
