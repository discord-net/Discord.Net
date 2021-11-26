namespace Discord
{
    /// <summary>
    ///     Represent a permission object.
    /// </summary>
    public struct Overwrite
    {
        /// <summary>
        ///     Gets the unique identifier for the object this overwrite is targeting.
        /// </summary>
        public ulong TargetId { get; }
        /// <summary>
        ///     Gets the type of object this overwrite is targeting.
        /// </summary>
        public PermissionTarget TargetType { get; }
        /// <summary>
        ///     Gets the permissions associated with this overwrite entry.
        /// </summary>
        public OverwritePermissions Permissions { get; }

        /// <summary>
        ///     Initializes a new <see cref="Overwrite"/> with provided target information and modified permissions.
        /// </summary>
        public Overwrite(ulong targetId, PermissionTarget targetType, OverwritePermissions permissions)
        {
            TargetId = targetId;
            TargetType = targetType;
            Permissions = permissions;
        }
    }
}
