namespace Discord
{
    /// <summary>
    ///     Properties that are used to reorder an <see cref="IRole" /> .
    /// </summary>
    public class ReorderRoleProperties
    {
        /// <summary>
        ///     Gets the ID of the role to be edited.
        /// </summary>
        public ulong Id { get; }
        /// <summary>
        ///     Gets the new zero-based position of the role.
        /// </summary>
        public int Position { get; }

        public ReorderRoleProperties(ulong id, int pos)
        {
            Id = id;
            Position = pos;
        }
    }
}
