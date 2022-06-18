using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API
{
    internal class ForumTags
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("emoji_id")]
        public Optional<ulong?> EmojiId { get; set; }
        [JsonProperty("emoji_name")]
        public Optional<string> EmojiName { get; set; }
    }
}
