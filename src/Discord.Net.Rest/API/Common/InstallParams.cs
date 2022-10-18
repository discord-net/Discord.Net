using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API
{
    internal class InstallParams
    {
        [JsonPropertyName("scopes")]
        public string[] Scopes { get; set; }
        [JsonPropertyName("permissions")]
        public ulong Permission { get; set; }
    }
}
