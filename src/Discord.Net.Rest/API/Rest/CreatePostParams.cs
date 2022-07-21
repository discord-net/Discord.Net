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

        [JsonProperty("message")]
        public ForumThreadMessage Message { get; set; }
    }
}
