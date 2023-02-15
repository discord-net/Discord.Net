using System;

namespace Discord
{
    /// <summary>
    ///     Represents a Discord thread user.
    /// </summary>
    public interface IThreadUser : IMentionable
    {
        /// <summary>
        ///     Gets the <see cref="IThreadChannel"/> this user is in.
        /// </summary>
        IThreadChannel Thread { get; }

        /// <summary>
        ///     Gets the timestamp for when this user joined this thread.
        /// </summary>
        DateTimeOffset ThreadJoinedAt { get; }

        /// <summary>
        ///     Gets the guild this thread was created in.
        /// </summary>
        IGuild Guild { get; }

        /// <summary>
        ///     Gets the <see cref="IGuildUser"/> on the server this thread was created in.
        /// </summary>
        IGuildUser GuildUser { get; }
    }
}
