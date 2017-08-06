#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rest
{
    internal class GetGuildPruneCountResponse
    {
        [ModelProperty("pruned")]
        public int Pruned { get; set; }
    }
}
