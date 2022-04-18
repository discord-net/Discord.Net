using System.Collections;
using System.Collections.Generic;

namespace Discord
{
    /// <summary>
    ///     Represents the base class to create/modify application commands.
    /// </summary>
    public abstract class ApplicationCommandProperties
    {
        internal abstract ApplicationCommandType Type { get; }

        /// <summary>
        ///     Gets or sets the name of this command.
        /// </summary>
        public Optional<string> Name { get; set; }

        /// <summary>
        ///     Gets or sets whether the command is enabled by default when the app is added to a guild. Default is <see langword="true"/>
        /// </summary>
        public Optional<bool> IsDefaultPermission { get; set; }

        public IDictionary<string, string>? NameLocalizations { get; set; }

        public IDictionary<string, string>? DescriptionLocalizations { get; set; }

        internal ApplicationCommandProperties() { }
    }
}
