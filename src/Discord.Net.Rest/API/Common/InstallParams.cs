using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API
{
    internal class InstallParams
    {
        [JsonProperty("scopes")]
        public string[] Scopes { get; set; }
        [JsonProperty("permissions")]
        public ulong Permission { get; set; }
    }
}
