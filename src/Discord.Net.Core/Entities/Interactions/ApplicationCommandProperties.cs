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
    /// <see cref="Ia"/>
    public class ApplicationCommandProperties
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Optional<IEnumerable<IApplicationCommandOption>> Options { get; set; }
    }
}
