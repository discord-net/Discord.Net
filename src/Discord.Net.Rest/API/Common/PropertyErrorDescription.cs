using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API
{
    internal class ErrorDetails
    {
        [JsonProperty("name")]
        public Optional<string> Name { get; set; }
        [JsonProperty("errors")]
        public Error[] Errors { get; set; }
    }
}
