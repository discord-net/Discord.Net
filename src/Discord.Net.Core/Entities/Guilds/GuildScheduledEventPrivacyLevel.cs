using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents the privacy level of a guild scheduled event.
    /// </summary>
    public enum GuildScheduledEventPrivacyLevel
    {
        /// <summary>
        ///     The scheduled event is public and available in discovery.
        /// </summary>
        [Obsolete("This event type isn't supported yet! check back later.", true)]
        Public = 1,

        /// <summary>
        ///     The scheduled event is only accessible to guild members.
        /// </summary>
        Private = 2,
    }
}
