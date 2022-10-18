using System.Text.Json.Serialization;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ModifyGuildWidgetParams
    {
        [JsonPropertyName("enabled")]
        public Optional<bool> Enabled { get; set; }
        [JsonPropertyName("channel")]
        public Optional<ulong?> ChannelId { get; set; }
    }
}
