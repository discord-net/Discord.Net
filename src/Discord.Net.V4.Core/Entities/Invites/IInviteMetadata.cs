using System;

namespace Discord
{
    /// <summary>
    ///     Represents additional information regarding the generic invite object.
    /// </summary>
    public interface IInviteMetadata : IInvite
    {
        /// <summary>
        ///     Gets a value that indicates whether the invite is a temporary one.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> if users accepting this invite will be removed from the guild when they log off; otherwise 
        ///     <see langword="false" />.
        /// </returns>
        bool IsTemporary { get; }
        /// <summary>
        ///     Gets the time (in seconds) until the invite expires.
        /// </summary>
        /// <returns>
        ///     An <see cref="int"/> representing the time in seconds until this invite expires; <see langword="null" /> if this
        ///     invite never expires.
        /// </returns>
        int? MaxAge { get; }
        /// <summary>
        ///     Gets the max number of uses this invite may have.
        /// </summary>
        /// <returns>
        ///     An <see cref="int"/> representing the number of uses this invite may be accepted until it is removed
        ///     from the guild; <see langword="null" /> if none is set.
        /// </returns>
        int? MaxUses { get; }
        /// <summary>
        ///     Gets the number of times this invite has been used.
        /// </summary>
        /// <returns>
        ///     An <see cref="int"/> representing the number of times this invite has been used.
        /// </returns>
        int? Uses { get; }
        /// <summary>
        ///     Gets when this invite was created.
        /// </summary>
        /// <returns>
        ///     A <see cref="DateTimeOffset"/> representing the time of which the invite was first created.
        /// </returns>
        DateTimeOffset? CreatedAt { get; }
    }
}
