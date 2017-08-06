#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rest
{
    internal class ModifyGuildChannelParams
    {
        [ModelProperty("name")]
        public Optional<string> Name { get; set; }
        [ModelProperty("position")]
        public Optional<int> Position { get; set; }
    }
}
