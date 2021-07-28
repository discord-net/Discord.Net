using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API
{
    internal class ThreadMetadata
    {
        [JsonProperty("archived")]
        public bool Archived { get; set; }

        [JsonProperty("auto_archive_duration")]
        public int AutoArchiveDuration { get; set; }

        [JsonProperty("archive_timestamp")]
        public DateTimeOffset ArchiveTimestamp { get; set; }

        [JsonProperty("locked")]
        public Optional<bool> Locked { get; set; }
    }
}
