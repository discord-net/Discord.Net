using System.Text.Json.Serialization;
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
        [JsonPropertyName("name")]
        public string Title { get; set; }

        [JsonPropertyName("auto_archive_duration")]
        public ThreadArchiveDuration ArchiveDuration { get; set; }

        [JsonPropertyName("rate_limit_per_user")]
        public Optional<int?> Slowmode { get; set; }

        [JsonPropertyName("message")]
        public ForumThreadMessage Message { get; set; }
    }
}
