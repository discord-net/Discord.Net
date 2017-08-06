#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rest
{
    internal class GuildPruneParams
    {
        [ModelProperty("days")]
        public int Days { get; }

        public GuildPruneParams(int days)
        {
            Days = days;
        }
    }
}
