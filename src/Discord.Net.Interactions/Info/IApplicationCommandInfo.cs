using System;
using System.Collections.Generic;

namespace Discord.Interactions
{
    /// <summary>
    ///     Represents a <see cref="InteractionService"/> command that can be registered to Discord.
    /// </summary>
    public interface IApplicationCommandInfo
    {
        /// <summary>
        ///     Gets the name of this command.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Gets the type of this command.
        /// </summary>
        ApplicationCommandType CommandType { get; }

        /// <summary>
        ///     Gets the DefaultPermission of this command.
        /// </summary>
        [Obsolete($"To be deprecated soon, use {nameof(IsEnabledInDm)} and {nameof(DefaultMemberPermissions)} instead.")]
        bool DefaultPermission { get; }

        /// <summary>
        ///     Gets whether this command can be used in DMs.
        /// </summary>
        public bool IsEnabledInDm { get; }

        /// <summary>
        ///     Gets whether this command can is age restricted.
        /// </summary>
        public bool IsNsfw { get; }

        /// <summary>
        ///     Gets the default permissions needed for executing this command.
        /// </summary>
        public GuildPermission? DefaultMemberPermissions { get; }

        /// <summary>
        ///     Gets the context types this command can be executed in.
        /// </summary>
        public IReadOnlyCollection<InteractionContextType> ContextTypes { get; }

        /// <summary>
        ///     Gets the install methods for this command.
        /// </summary>
        public IReadOnlyCollection<ApplicationIntegrationType> IntegrationTypes { get; }
    }
}
