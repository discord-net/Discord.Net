using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API
{
    [JsonConverter(typeof(Discord.Net.Converters.DiscordErrorConverter))]
    internal class DiscordError
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }
        [JsonPropertyName("code")]
        public DiscordErrorCode Code { get; set; }
        [JsonPropertyName("errors")]
        public Optional<ErrorDetails[]> Errors { get; set; }
    }
}
