using Newtonsoft.Json;
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
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("code")]
        public DiscordErrorCode Code { get; set; }
        [JsonProperty("errors")]
        public Optional<ErrorDetails[]> Errors { get; set; }
    }
}
