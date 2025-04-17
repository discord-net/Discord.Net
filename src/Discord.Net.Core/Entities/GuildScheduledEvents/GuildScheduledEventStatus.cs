using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents the status of a guild event.
    /// </summary>
    public enum GuildScheduledEventStatus
    {
        /// <summary>
        ///     The event is scheduled for a set time.
        /// </summary>
        Scheduled = 1,

        /// <summary>
        ///     The event has started.
        /// </summary>
        Active = 2,

        /// <summary>
        ///     The event was completed.
        /// </summary>
        Completed = 3,

        /// <summary>
        ///     The event was canceled.
        /// </summary>
        Cancelled = 4,
    }
}
