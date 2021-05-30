using Newtonsoft.Json;
using System.Collections.Generic;

namespace Discord.API
{
    internal class ApplicationCommandInteractionDataResolved
    {
        [JsonProperty("users")]
        public Optional<Dictionary<ulong, User>> Users { get; set; }

        [JsonProperty("members")]
        public Optional<Dictionary<ulong, GuildMember>> Members { get; set; }

        [JsonProperty("channels")]
        public Optional<Dictionary<ulong, Channel>> Channels { get; set; }

        [JsonProperty("roles")]
        public Optional<Dictionary<ulong, Role>> Roles { get; set; }
    }
}
