#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rest
{
    internal class CreateMessageParams
    {
        [ModelProperty("content")]
        public string Content { get; }

        [ModelProperty("nonce")]
        public Optional<string> Nonce { get; set; }
        [ModelProperty("tts")]
        public Optional<bool> IsTTS { get; set; }
        [ModelProperty("embed")]
        public Optional<Embed> Embed { get; set; }

        public CreateMessageParams(string content)
        {
            Content = content;
        }
    }
}
