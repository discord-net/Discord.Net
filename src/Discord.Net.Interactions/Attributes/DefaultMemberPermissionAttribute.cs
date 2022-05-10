using System;

namespace Discord.Interactions
{
    /// <summary>
    ///     Sets the <see cref="IApplicationCommandInfo.DefaultMemberPermissions"/> of an application command or module.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class DefaultMemberPermissionsAttribute : Attribute
    {
        /// <summary>
        ///     Gets the default permission required to use this command.
        /// </summary>
        public GuildPermission Permissions { get; }

        /// <summary>
        ///     Sets the <see cref="IApplicationCommandInfo.DefaultMemberPermissions"/> of an application command or module.
        /// </summary>
        /// <param name="permissions">The default permission required to use this command.</param>
        public DefaultMemberPermissionsAttribute(GuildPermission permissions)
        {
            Permissions = permissions;
        }
    }
}
