using Newtonsoft.Json;
using System.Collections.Generic;

namespace Discord.API
{
    internal class ApplicationCommandInteractionDataResolved
    {
        [JsonProperty("users")]
        public Optional<Dictionary<string, User>> Users { get; set; }

        [JsonProperty("members")]
        public Optional<Dictionary<string, GuildMember>> Members { get; set; }

        [JsonProperty("channels")]
        public Optional<Dictionary<string, Channel>> Channels { get; set; }

        [JsonProperty("roles")]
        public Optional<Dictionary<string, Role>> Roles { get; set; }
        [JsonProperty("messages")]
        public Optional<Dictionary<string, Message>> Messages { get; set; }
        [JsonProperty("attachments")]
        public Optional<Dictionary<string, Attachment>> Attachments { get; set; }
    }
}
