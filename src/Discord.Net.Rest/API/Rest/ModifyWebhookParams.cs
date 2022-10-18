using System.Text.Json.Serialization;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ModifyWebhookParams
    {
        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }
        [JsonPropertyName("avatar")]
        public Optional<Image?> Avatar { get; set; }
        [JsonPropertyName("channel_id")]
        public Optional<ulong> ChannelId { get; set; }
    }
}
