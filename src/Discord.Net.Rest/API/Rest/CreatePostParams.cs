using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API.Rest
{
    internal class CreatePostParams
    {
        // thread
        [JsonProperty("name")]
        public string Title { get; set; }

        [JsonProperty("auto_archive_duration")]
        public ThreadArchiveDuration ArchiveDuration { get; set; }

        [JsonProperty("rate_limit_per_user")]
        public Optional<int?> Slowmode { get; set; }

        // message
        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("tts")]
        public Optional<bool> IsTTS { get; set; }

        [JsonProperty("embeds")]
        public Optional<Embed[]> Embeds { get; set; }

        [JsonProperty("allowed_mentions")]
        public Optional<AllowedMentions> AllowedMentions { get; set; }

        [JsonProperty("components")]
        public Optional<API.ActionRowComponent[]> Components { get; set; }

        [JsonProperty("sticker_ids")]
        public Optional<ulong[]> Stickers { get; set; }

        [JsonProperty("flags")]
        public Optional<MessageFlags> Flags { get; set; }
    }
}
