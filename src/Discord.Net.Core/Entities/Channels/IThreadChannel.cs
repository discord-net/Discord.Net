using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a thread channel inside of a guild.
    /// </summary>
    public interface IThreadChannel : ITextChannel, IGuildChannel
    {
        /// <summary>
        ///     <see langword="true"/> if the current thread is archived, otherwise <see langword="false"/>.
        /// </summary>
        bool Archived { get; }

        /// <summary>
        ///     Duration to automatically archive the thread after recent activity.
        /// </summary>
        ThreadArchiveDuration AutoArchiveDuration { get; }

        /// <summary>
        ///     Timestamp when the thread's archive status was last changed, used for calculating recent activity.
        /// </summary>
        DateTimeOffset ArchiveTimestamp { get; }

        /// <summary>
        ///     <see langword="true"/> if the current thread is locked, otherwise <see langword="false"/>
        /// </summary>
        bool Locked { get; }

        /// <summary>
        ///     An approximate count of users in a thread, stops counting at 50.
        /// </summary>
        int MemberCount { get; }

        /// <summary>
        ///     An approximate count of messages in a thread, stops counting at 50.
        /// </summary>
        int MessageCount { get; }
    }
}
