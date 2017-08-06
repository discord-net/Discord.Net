#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rest
{
    internal class ModifyTextChannelParams : ModifyGuildChannelParams
    {
        [ModelProperty("topic")]
        public Optional<string> Topic { get; set; }
    }
}
