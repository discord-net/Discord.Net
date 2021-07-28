using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API.Rest
{
    internal class ModifyGuildApplicationCommandPermissionsParams
    {
        [JsonProperty("permissions")]
        public ApplicationCommandPermissions[] Permissions { get; set; }
    }
}
