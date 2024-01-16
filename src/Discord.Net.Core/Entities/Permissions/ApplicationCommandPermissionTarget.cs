namespace Discord
{
    /// <summary>
    ///     Specifies the target of the permission.
    /// </summary>
    public enum ApplicationCommandPermissionTarget
    {
        /// <summary>
        ///     The target of the permission is a role.
        /// </summary>
        Role = 1,
        /// <summary>
        ///     The target of the permission is a user.
        /// </summary>
        User = 2,
        /// <summary>
        ///     The target of the permission is a channel.
        /// </summary>
        Channel = 3
    }
}
