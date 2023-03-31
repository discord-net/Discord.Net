using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API.Voice;
internal class ClientDisconnectEvent
{
    [JsonProperty("user_id")]
    public ulong UserId { get; set; }
}
