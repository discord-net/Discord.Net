using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API
{
    internal class ErrorDetails
    {
        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }
        [JsonPropertyName("errors")]
        public Error[] Errors { get; set; }
    }
}
