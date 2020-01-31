using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API.Rest
{
    class CreateChannelPermissionsParams
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("type")]
        public string Type { get; }
        [JsonProperty("allow")]
        public ulong Allow { get; }
        [JsonProperty("deny")]
        public ulong Deny { get; }

        public CreateChannelPermissionsParams(ulong id, string type, ulong allow, ulong deny)
        {
            Id = id;
            Type = type;
            Allow = allow;
            Deny = deny;
        }
    }
}
