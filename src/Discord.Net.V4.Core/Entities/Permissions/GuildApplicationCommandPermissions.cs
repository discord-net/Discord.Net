using System.Collections.Generic;

namespace Discord
{
    /// <summary>
    ///     Returned when fetching the permissions for a command in a guild.
    /// </summary>
    public class GuildApplicationCommandPermission

    {
        /// <summary>
        ///     The id of the command.
        /// </summary>
        public ulong CommandId { get; }

        /// <summary>
        ///     The id of the application the command belongs to.
        /// </summary>
        public ulong ApplicationId { get; }

        /// <summary>
        ///     The id of the guild.
        /// </summary>
        public ulong GuildId { get; }

        /// <summary>
        ///     The permissions for the command in the guild.
        /// </summary>
        public IReadOnlyCollection<ApplicationCommandPermission> Permissions { get; }

        internal GuildApplicationCommandPermission(ulong commandId, ulong appId, ulong guildId, ApplicationCommandPermission[] permissions)
        {
            CommandId = commandId;
            ApplicationId = appId;
            GuildId = guildId;
            Permissions = permissions;
        }
    }
}
