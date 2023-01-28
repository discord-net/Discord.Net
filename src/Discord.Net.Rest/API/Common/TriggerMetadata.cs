using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API
{
    internal class TriggerMetadata
    {
        [JsonProperty("keyword_filter")]
        public string[] KeywordFilter { get; set; }

        [JsonProperty("presets")]
        public KeywordPresetTypes[] Presets { get; set; }
    }
}
