namespace Discord
{
    public class BulkRoleProperties : RoleProperties
    {
        /// <summary>
        /// The id of the role to be edited
        /// </summary>
        public ulong Id { get; }

        public BulkRoleProperties(ulong id)
        {
            Id = id;
        }
    }
}
