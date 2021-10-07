using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///    Represents the types of application commands.
    /// </summary>
    public enum ApplicationCommandType : byte
    {
        /// <summary>
        ///     A Slash command type
        /// </summary>
        Slash = 1,

        /// <summary>
        ///     A Context Menu User command type
        /// </summary>
        User = 2,

        /// <summary>
        ///     A Context Menu Message command type
        /// </summary>
        Message = 3
    }
}
