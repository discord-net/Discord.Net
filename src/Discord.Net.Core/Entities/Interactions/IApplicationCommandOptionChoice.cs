using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Specifies choices for command group.
    /// </summary>
    public interface IApplicationCommandOptionChoice
    {
        /// <summary>
        ///    Gets the name of this choice.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Gets the value of the choice.
        /// </summary>
        object Value { get; }

    }
}
