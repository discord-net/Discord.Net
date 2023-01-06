namespace Discord
{
    /// <summary>
    ///     Application command permissions allow you to enable or disable commands for specific users or roles within a guild.
    /// </summary>
    public class ApplicationCommandPermission
    {
        /// <summary>
        ///     The id of the role or user.
        /// </summary>
        public ulong TargetId { get; }

        /// <summary>
        ///     The target of this permission.
        /// </summary>
        public ApplicationCommandPermissionTarget TargetType { get; }

        /// <summary>
        ///     <see langword="true"/> to allow, otherwise <see langword="false"/>.
        /// </summary>
        public bool Permission { get; }

        internal ApplicationCommandPermission() { }

        /// <summary>
        ///     Creates a new <see cref="ApplicationCommandPermission"/>.
        /// </summary>
        /// <param name="targetId">The id you want to target this permission value for.</param>
        /// <param name="targetType">The type of the <b>targetId</b> parameter.</param>
        /// <param name="allow">The value of this permission.</param>
        public ApplicationCommandPermission(ulong targetId, ApplicationCommandPermissionTarget targetType, bool allow)
        {
            TargetId = targetId;
            TargetType = targetType;
            Permission = allow;
        }

        /// <summary>
        ///     Creates a new <see cref="ApplicationCommandPermission"/> targeting <see cref="ApplicationCommandPermissionTarget.User"/>.
        /// </summary>
        /// <param name="target">The user you want to target this permission value for.</param>
        /// <param name="allow">The value of this permission.</param>
        public ApplicationCommandPermission(IUser target, bool allow)
        {
            TargetId = target.Id;
            Permission = allow;
            TargetType = ApplicationCommandPermissionTarget.User;
        }

        /// <summary>
        ///     Creates a new <see cref="ApplicationCommandPermission"/> targeting <see cref="ApplicationCommandPermissionTarget.Role"/>.
        /// </summary>
        /// <param name="target">The role you want to target this permission value for.</param>
        /// <param name="allow">The value of this permission.</param>
        public ApplicationCommandPermission(IRole target, bool allow)
        {
            TargetId = target.Id;
            Permission = allow;
            TargetType = ApplicationCommandPermissionTarget.Role;
        }
    }
}
