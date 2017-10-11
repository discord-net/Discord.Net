#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ModifyWebhookParams
    {
        [JsonProperty("name")]
        public Optional<string> Name { get; set; }
        [JsonProperty("avatar")]
        public Optional<Image?> Avatar { get; set; }
        [JsonProperty("channel_id")]
        public Optional<ulong> ChannelId { get; set; }
    }
}
