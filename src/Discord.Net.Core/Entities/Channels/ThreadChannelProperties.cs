using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Entities
{
    public class ThreadChannelProperties
    {
        /// <summary>
        ///     Gets or sets whether or not the thread is archived.
        /// </summary>
        public Optional<bool> Archived { get; set; }

        /// <summary>
        ///     Gets or sets whether or not the thread is locked.
        /// </summary>
        public Optional<bool> Locked { get; set; }

        /// <summary>
        ///     Gets or sets the name of the thread.
        /// </summary>
        public Optional<string> Name { get; set; }

        /// <summary>
        ///     Gets or sets the auto archive duration.
        /// </summary>
        public Optional<ThreadArchiveDuration> AutoArchiveDuration { get; set; }

        /// <summary>
        ///     Gets or sets the slow-mode ratelimit in seconds for this channel.
        /// </summary>
        /// <remarks>
        ///     Setting this value to anything above zero will require each user to wait X seconds before
        ///     sending another message; setting this value to <c>0</c> will disable slow-mode for this channel.
        ///     <note>
        ///         Users with <see cref="Discord.ChannelPermission.ManageMessages"/> or 
        ///         <see cref="ChannelPermission.ManageChannels"/> will be exempt from slow-mode.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the value does not fall within [0, 21600].</exception>
        public Optional<int> SlowModeInterval { get; set; }
    }
}
