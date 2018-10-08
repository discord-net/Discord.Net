namespace Discord
{
    /// <summary>
    ///     Represents a user's voice connection status.
    /// </summary>
    public interface IVoiceState
    {
        /// <summary>
        ///     Gets a value that indicates whether this user is deafened by the guild.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if the user is deafened (i.e. not permitted to listen to or speak to others) by the guild;
        ///     otherwise <c>false</c>.
        /// </returns>
        bool IsDeafened { get; }
        /// <summary>
        ///     Gets a value that indicates whether this user is muted (i.e. not permitted to speak via voice) by the
        ///     guild.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if this user is muted by the guild; otherwise <c>false</c>.
        /// </returns>
        bool IsMuted { get; }
        /// <summary>
        ///     Gets a value that indicates whether this user has marked themselves as deafened.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if this user has deafened themselves (i.e. not permitted to listen to or speak to others); otherwise <c>false</c>.
        /// </returns>
        bool IsSelfDeafened { get; }
        /// <summary>
        ///     Gets a value that indicates whether this user has marked themselves as muted (i.e. not permitted to
        ///     speak via voice).
        /// </summary>
        /// <returns>
        ///     <c>true</c> if this user has muted themselves; otherwise <c>false</c>.
        /// </returns>
        bool IsSelfMuted { get; }
        /// <summary>
        ///     Gets a value that indicates whether the user is muted by the current user.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if the guild is temporarily blocking audio to/from this user; otherwise <c>false</c>.
        /// </returns>
        bool IsSuppressed { get; }
        /// <summary>
        ///     Gets the voice channel this user is currently in.
        /// </summary>
        /// <returns>
        ///     A generic voice channel object representing the voice channel that the user is currently in; <c>null</c>
        ///     if none.
        /// </returns>
        IVoiceChannel VoiceChannel { get; }
        /// <summary>
        ///     Gets the unique identifier for this user's voice session.
        /// </summary>
        string VoiceSessionId { get; }
    }
}
