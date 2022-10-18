using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API
{
    internal class ForumThreadMessage
    {
        [JsonPropertyName("content")]
        public Optional<string> Content { get; set; }

        [JsonPropertyName("nonce")]
        public Optional<string> Nonce { get; set; }

        [JsonPropertyName("embeds")]
        public Optional<Embed[]> Embeds { get; set; }

        [JsonPropertyName("allowed_mentions")]
        public Optional<AllowedMentions> AllowedMentions { get; set; }

        [JsonPropertyName("components")]
        public Optional<API.ActionRowComponent[]> Components { get; set; }

        [JsonPropertyName("sticker_ids")]
        public Optional<ulong[]> Stickers { get; set; }

        [JsonPropertyName("flags")]
        public Optional<MessageFlags> Flags { get; set; }
    }
}
