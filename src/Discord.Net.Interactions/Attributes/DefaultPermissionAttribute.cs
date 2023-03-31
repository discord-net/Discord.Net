using System;

namespace Discord.Interactions
{
    /// <summary>
    ///     Set the "Default Permission" property of an Application Command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    [Obsolete($"Soon to be deprecated, use Permissions-v2 attributes like {nameof(EnabledInDmAttribute)} and {nameof(DefaultMemberPermissionsAttribute)}")]
    public class DefaultPermissionAttribute : Attribute
    {
        /// <summary>
        ///     Gets whether the users are allowed to use a Slash Command by default or not.
        /// </summary>
        public bool IsDefaultPermission { get; }

        /// <summary>
        ///     Set the default permission of a Slash Command.
        /// </summary>
        /// <param name="isDefaultPermission"><see langword="true"/> if the users are allowed to use this command.</param>
        public DefaultPermissionAttribute(bool isDefaultPermission)
        {
            IsDefaultPermission = isDefaultPermission;
        }
    }
}
