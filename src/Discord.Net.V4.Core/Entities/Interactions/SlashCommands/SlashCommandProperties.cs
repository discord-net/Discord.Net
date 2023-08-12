using System.Collections.Generic;

namespace Discord
{
    /// <summary>
    ///     Represents a class used to create slash commands.
    /// </summary>
    public class SlashCommandProperties : ApplicationCommandProperties
    {
        internal override ApplicationCommandType Type => ApplicationCommandType.Slash;

        /// <summary>
        ///    Gets or sets the description of this command.
        /// </summary>
        public Optional<string> Description { get; set; }

        /// <summary>
        ///     Gets or sets the options for this command.
        /// </summary>
        public Optional<List<ApplicationCommandOptionProperties>> Options { get; set; }

        internal SlashCommandProperties() { }
    }
}
