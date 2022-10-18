using System.Text.Json.Serialization;

namespace Discord.API.Gateway
{
    internal class RecipientEvent
    {
        [JsonPropertyName("user")]
        public User User { get; set; }
        [JsonPropertyName("channel_id")]
        public ulong ChannelId { get; set; }
    }
}
