using System.Text.Json.Serialization;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ModifyWebhookMessageParams
    {
        [JsonPropertyName("content")]
        public Optional<string> Content { get; set; }
        [JsonPropertyName("embeds")]
        public Optional<Embed[]> Embeds { get; set; }
        [JsonPropertyName("allowed_mentions")]
        public Optional<AllowedMentions> AllowedMentions { get; set; }
        [JsonPropertyName("components")]
        public Optional<API.ActionRowComponent[]> Components { get; set; }
    }
}
