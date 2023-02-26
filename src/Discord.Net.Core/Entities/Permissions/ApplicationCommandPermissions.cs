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

        /// <summary>
        ///     Creates a new <see cref="ApplicationCommandPermission"/> targeting <see cref="ApplicationCommandPermissionTarget.Channel"/>.
        /// </summary>
        /// <param name="channel">The channel you want to target this permission value for.</param>
        /// <param name="allow">The value of this permission.</param>
        public ApplicationCommandPermission(IChannel channel, bool allow)
        {
            TargetId = channel.Id;
            Permission = allow;
            TargetType = ApplicationCommandPermissionTarget.Channel;
        }

        /// <summary>
        ///     Creates a new <see cref="ApplicationCommandPermission"/> targeting @everyone in a guild.
        /// </summary>
        /// <param name="guildId">Id of the target guild.</param>
        /// <param name="allow">The value of this permission.</param>
        /// <returns>
        ///     Instance of <see cref="ApplicationCommandPermission"/> targeting @everyone in a guild.
        /// </returns>
        public static ApplicationCommandPermission ForEveryone(ulong guildId, bool allow) =>
            new(guildId, ApplicationCommandPermissionTarget.User, allow);

        /// <summary>
        ///     Creates a new <see cref="ApplicationCommandPermission"/> targeting @everyone in a guild.
        /// </summary>
        /// <param name="guild">Target guild.</param>
        /// <param name="allow">The value of this permission.</param>
        /// <returns>
        ///     Instance of <see cref="ApplicationCommandPermission"/> targeting @everyone in a guild.
        /// </returns>
        public static ApplicationCommandPermission ForEveryone(IGuild guild, bool allow) =>
            ForEveryone(guild.Id, allow);

        /// <summary>
        ///     Creates a new <see cref="ApplicationCommandPermission"/> targeting every channel in a guild.
        /// </summary>
        /// <param name="guildId">Id of the target guild.</param>
        /// <param name="allow">The value of this permission.</param>
        /// <returns>
        ///     Instance of <see cref="ApplicationCommandPermission"/> targeting every channel in a guild.
        /// </returns>
        public static ApplicationCommandPermission ForAllChannels(ulong guildId, bool allow) =>
            new(guildId - 1, ApplicationCommandPermissionTarget.Channel, allow);

        /// <summary>
        ///     Creates a new <see cref="ApplicationCommandPermission"/> targeting every channel in a guild.
        /// </summary>
        /// <param name="guild">Target guild.</param>
        /// <param name="allow">The value of this permission.</param>
        /// <returns>
        ///     Instance of <see cref="ApplicationCommandPermission"/> targeting every channel in a guild.
        /// </returns>
        public static ApplicationCommandPermission ForAllChannels(IGuild guild, bool allow) =>
            ForAllChannels(guild.Id, allow);
    }
}
