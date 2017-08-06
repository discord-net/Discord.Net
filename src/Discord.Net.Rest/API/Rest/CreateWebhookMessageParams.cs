#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rest
{
    internal class CreateWebhookMessageParams
    {
        [ModelProperty("content")]
        public string Content { get; }

        [ModelProperty("nonce")]
        public Optional<string> Nonce { get; set; }
        [ModelProperty("tts")]
        public Optional<bool> IsTTS { get; set; }
        [ModelProperty("embeds")]
        public Optional<Embed[]> Embeds { get; set; }
        [ModelProperty("username")]
        public Optional<string> Username { get; set; }
        [ModelProperty("avatar_url")]
        public Optional<string> AvatarUrl { get; set; }

        public CreateWebhookMessageParams(string content)
        {
            Content = content;
        }
    }
}
