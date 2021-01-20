#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class EditWebhookMessageParams
    {
        [JsonProperty("content")]
        public string Content { get; }

        [JsonProperty("embeds")]
        public Optional<Embed[]> Embeds { get; set; }
        [JsonProperty("allowed_mentions")]
        public Optional<AllowedMentions> AllowedMentions { get; set; }

        public EditWebhookMessageParams(string content)
        {
            Content = content;
        }
    }
}
