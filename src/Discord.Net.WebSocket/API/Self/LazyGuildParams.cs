using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API.Self;

[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
internal class LazyGuildParams
{
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("typing")]
    public bool Typing { get; set; }

    [JsonProperty("threads")]
    public bool Threads { get; set; }

    [JsonProperty("activities")]
    public bool Activities { get; set; }

    [JsonProperty("members")]
    public string[] Members { get; set; }

    [JsonProperty("channels")]
    public Dictionary<ulong, int[][]> Channels { get; set; }
}
