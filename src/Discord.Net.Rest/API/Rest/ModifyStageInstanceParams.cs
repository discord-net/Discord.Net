using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API.Rest
{
    internal class ModifyStageInstanceParams
    {

        [JsonProperty("topic")]
        public Optional<string> Topic { get; set; }

        [JsonProperty("privacy_level")]
        public Optional<StagePrivacyLevel> PrivacyLevel { get; set; }
    }
}
