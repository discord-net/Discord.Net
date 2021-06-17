using Discord.Net.Models;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Represents a response to list <see cref="ThreadChannel"/>s.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/channel#list-active-threads-response-body"/>
    /// </remarks>
    public record ThreadList
    {
        /// <summary>
        /// Array of <see cref="ThreadChannel"/>s returned.
        /// </summary>
        public ThreadChannel[]? Threads { get; init; } // Required property candidate

        /// <summary>
        /// Array of <see cref="ThreadMember"/>s for each returned thread the current
        /// user has joined.
        /// </summary>
        public ThreadMember[]? Members { get; init; } // Required property candidate

        /// <summary>
        /// Whether there are potentially additional <see cref="ThreadChannel"/>s that
        /// could be returned on a subsequent call.
        /// </summary>
        public bool HasMore { get; init; }
    }
}
