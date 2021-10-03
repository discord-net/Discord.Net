using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     A class used to create slash commands.
    /// </summary>
    public class SlashCommandProperties : ApplicationCommandProperties
    {
        internal override ApplicationCommandType Type => ApplicationCommandType.Slash;
        /// <summary>
        ///    The description of this command.
        /// </summary>
        public Optional<string> Description { get; set; }

        /// <summary>
        ///     Gets or sets the options for this command.
        /// </summary>
        public Optional<List<ApplicationCommandOptionProperties>> Options { get; set; }

        internal SlashCommandProperties() { }
    }
}
