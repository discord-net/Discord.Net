using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public enum AutoModActionType
    {
        /// <summary>
        ///     Blocks the content of a message according to the rule.
        /// </summary>
        BlockMessage = 1,

        /// <summary>
        ///     Logs user content to a specified channel.
        /// </summary>
        SendAlertMessage = 2,

        /// <summary>
        ///     Timeout user for a specified duration.
        /// </summary>
        Timeout = 3,
    }
}
