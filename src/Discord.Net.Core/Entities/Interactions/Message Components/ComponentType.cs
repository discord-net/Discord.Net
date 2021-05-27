using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a type of a component
    /// </summary>
    public enum ComponentType
    {
        /// <summary>
        ///     A container for other components
        /// </summary>
        ActionRow = 1,

        /// <summary>
        ///     A clickable button
        /// </summary>
        Button = 2
    }
}
