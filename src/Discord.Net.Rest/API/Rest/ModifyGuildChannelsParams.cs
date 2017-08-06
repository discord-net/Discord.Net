#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rest
{
    internal class ModifyGuildChannelsParams
    {
        [ModelProperty("id")]
        public ulong Id { get; }
        [ModelProperty("position")]
        public int Position { get; }

        public ModifyGuildChannelsParams(ulong id, int position)
        {
            Id = id;
            Position = position;
        }
    }
}
