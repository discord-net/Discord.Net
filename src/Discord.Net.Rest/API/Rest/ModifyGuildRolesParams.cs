using System.Text.Json.Serialization;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ModifyGuildRolesParams : ModifyGuildRoleParams
    {
        [JsonPropertyName("id")]
        public ulong Id { get; }
        [JsonPropertyName("position")]
        public int Position { get; }

        public ModifyGuildRolesParams(ulong id, int position)
        {
            Id = id;
            Position = position;
        }
    }
}
