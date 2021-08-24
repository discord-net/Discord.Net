using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     A class used to create User commands.
    /// </summary>
    public class UserCommandProperties : ApplicationCommandProperties
    {
        internal override ApplicationCommandType Type => ApplicationCommandType.User;
    }
}
