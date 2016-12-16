namespace Discord
{
    public class ModifyGuildRolesParams : ModifyGuildRoleParams
    {
        /// <summary>
        /// The id of the role to be edited
        /// </summary>
        public ulong Id { get; }

        public ModifyGuildRolesParams(ulong id)
        {
            Id = id;
        }
    }
}
