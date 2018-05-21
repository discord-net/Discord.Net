namespace Discord
{
    /// <summary>
    ///     Represents a user's voice connection status.
    /// </summary>
    public interface IVoiceState
    {
        /// <summary>
        ///     Returns <c>true</c> if the guild has deafened this user.
        /// </summary>
        bool IsDeafened { get; }
        /// <summary>
        ///     Returns <c>true</c> if the guild has muted this user.
        /// </summary>
        bool IsMuted { get; }
        /// <summary>
        ///     Returns <c>true</c> if this user has marked themselves as deafened.
        /// </summary>
        bool IsSelfDeafened { get; }
        /// <summary>
        ///     Returns <c>true</c> if this user has marked themselves as muted.
        /// </summary>
        bool IsSelfMuted { get; }
        /// <summary>
        ///     Returns <c>true</c> if the guild is temporarily blocking audio to/from this user.
        /// </summary>
        bool IsSuppressed { get; }
        /// <summary>
        ///     Gets the voice channel this user is currently in, or <c>null</c> if none.
        /// </summary>
        IVoiceChannel VoiceChannel { get; }
        /// <summary>
        ///     Gets the unique identifier for this user's voice session.
        /// </summary>
        string VoiceSessionId { get; }
    }
}
