#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rest
{
    internal class ModifyGuildRolesParams : ModifyGuildRoleParams
    {
        [ModelProperty("id")]
        public ulong Id { get; }
        [ModelProperty("position")]
        public int Position { get; }

        public ModifyGuildRolesParams(ulong id, int position)
        {
            Id = id;
            Position = position;
        }
    }
}
