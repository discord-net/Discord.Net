namespace Discord
{
    public class ReorderRoleProperties
    {
        public ReorderRoleProperties(ulong id, int pos)
        {
            Id = id;
            Position = pos;
        }

        /// <summary>The id of the role to be edited</summary>
        public ulong Id { get; }

        /// <summary>The new zero-based position of the role.</summary>
        public int Position { get; }
    }
}
