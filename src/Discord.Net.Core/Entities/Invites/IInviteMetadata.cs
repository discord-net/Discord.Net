using System;

namespace Discord
{
    /// <summary> Represents additional information regarding the generic invite object. </summary>
    public interface IInviteMetadata : IInvite
    {
        /// <summary>
        ///     Gets the user that created this invite.
        /// </summary>
        IUser Inviter { get; }
        /// <summary>
        ///     Returns <c>true</c> if this invite was revoked.
        /// </summary>
        bool IsRevoked { get; }
        /// <summary>
        ///     Returns <c>true</c> if users accepting this invite will be removed from the guild when they
        ///     log off.
        /// </summary>
        bool IsTemporary { get; }
        /// <summary>
        ///     Gets the time (in seconds) until the invite expires, or <c>null</c> if it never expires.
        /// </summary>
        int? MaxAge { get; }
        /// <summary>
        ///     Gets the max amount of times this invite may be used, or <c>null</c> if there is no limit.
        /// </summary>
        int? MaxUses { get; }
        /// <summary>
        ///     Gets the amount of times this invite has been used.
        /// </summary>
        int Uses { get; }
        /// <summary>
        ///     Gets when this invite was created.
        /// </summary>
        DateTimeOffset CreatedAt { get; }
    }
}
