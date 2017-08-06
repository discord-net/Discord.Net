#pragma warning disable CS1591
using Discord.Serialization;
using System.Collections.Generic;

namespace Discord.API.Rpc
{
    internal class Guild
    {
        [ModelProperty("id")]
        public ulong Id { get; set; }
        [ModelProperty("name")]
        public string Name { get; set; }
        [ModelProperty("icon_url")]
        public string IconUrl { get; set; }
        [ModelProperty("members")]
        public IEnumerable<GuildMember> Members { get; set; }
    }
}
