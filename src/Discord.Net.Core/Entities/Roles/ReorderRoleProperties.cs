namespace Discord
{
    public class ReorderRoleProperties
    {
        /// <summary>The id of the role to be edited</summary>
        public ulong Id { get; }
        /// <summary>The new zero-based position of the role.</summary>
        public int Position { get; }

        public ReorderRoleProperties(ulong id, int pos)
        {
            Id = id;
            Position = pos;
        }
    }
}
