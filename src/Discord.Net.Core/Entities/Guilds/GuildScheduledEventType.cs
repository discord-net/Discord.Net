using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents the type of a guild scheduled event.
    /// </summary>
    public enum GuildScheduledEventType
    {
        /// <summary>
        ///     The event doesn't have a set type.
        /// </summary>
        None = 0,

        /// <summary>
        ///     The event is set in a stage channel.
        /// </summary>
        Stage = 1,

        /// <summary>
        ///     The event is set in a voice channel.
        /// </summary>
        Voice = 2,

        /// <summary>
        ///     The event is set for somewhere externally from discord.
        /// </summary>
        External = 3,
    }
}
