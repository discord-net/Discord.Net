using System.Text.Json.Serialization;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class CreateWebhookParams
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("avatar")]
        public Optional<Image?> Avatar { get; set; }
    }
}
