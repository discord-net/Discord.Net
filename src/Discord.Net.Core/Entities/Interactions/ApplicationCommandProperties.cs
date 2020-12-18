using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Provides properties that are used to modify a <see cref="IApplicationCommand" /> with the specified changes.
    /// </summary>
    public class ApplicationCommandProperties
    {
        /// <summary>
        ///     Gets or sets the name of this command.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the discription of this command.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Gets or sets the options for this command.
        /// </summary>
        public Optional<IEnumerable<IApplicationCommandOption>> Options { get; set; }
    }
}
