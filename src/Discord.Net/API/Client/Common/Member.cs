using Discord.API.Converters;
using Newtonsoft.Json;
using System;

namespace Discord.API.Client
{
    public class Member : MemberReference
    {
        [JsonProperty("joined_at")]
        public DateTime? JoinedAt { get; set; }
        [JsonProperty("roles"), JsonConverter(typeof(LongStringArrayConverter))]
        public ulong[] Roles { get; set; }
        [JsonProperty("nick")]
        public string Nick { get; set; } = "";
    }
}
