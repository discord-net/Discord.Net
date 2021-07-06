using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API
{
    internal class GuildApplicationCommandPermission
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonProperty("application_id")]
        public ulong ApplicationId { get; set; }

        [JsonProperty("guild_id")]
        public ulong GuildId { get; set; }

        [JsonProperty("permissions")]
        public API.ApplicationCommandPermissions[] Permissions { get; set; }
    }
}
