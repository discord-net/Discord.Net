using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API.Gateway
{
    internal class ApplicationCommandCreatedUpdatedEvent : API.ApplicationCommand
    {
        [JsonProperty("guild_id")]
        public Optional<ulong> GuildId { get; set; }
    }
}
