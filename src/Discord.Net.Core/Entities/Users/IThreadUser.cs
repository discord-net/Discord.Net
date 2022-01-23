using System;

namespace Discord
{
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

        /// <inheritdoc />
        string Mention { get; }
    }
}
