using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        internal ApplicationCommandProperties() { }
    }
}
