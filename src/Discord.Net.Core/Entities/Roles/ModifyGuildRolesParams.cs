namespace Discord
{
    public class ModifyGuildRolesParams : ModifyGuildRoleParams
    {
        public ulong Id { get; }

        public ModifyGuildRolesParams(ulong id)
        {
            Id = id;
        }
    }
}
