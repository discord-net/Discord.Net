using System;

namespace Discord
{
    public interface IInviteMetadata : IInvite
    {
        /// <summary> Gets the user that created this invite. </summary>
        IUser Inviter { get; }
        /// <summary> Returns true if this invite was revoked. </summary>
        bool IsRevoked { get; }
        /// <summary> Returns true if users accepting this invite will be removed from the guild when they log off. </summary>
        bool IsTemporary { get; }
        /// <summary> Gets the time (in seconds) until the invite expires, or null if it never expires. </summary>
        int? MaxAge { get; }
        /// <summary> Gets the max amount of times this invite may be used, or null if there is no limit. </summary>
        int? MaxUses { get; }
        /// <summary> Gets the amount of times this invite has been used. </summary>
        int Uses { get; }
        /// <summary> Gets when this invite was created. </summary>
        DateTimeOffset CreatedAt { get; }
    }
}