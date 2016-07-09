namespace Discord
{
    public interface IVoiceState
    {
        /// <summary> Returns true if the guild has deafened this user. </summary>
        bool IsDeafened { get; }
        /// <summary> Returns true if the guild has muted this user. </summary>
        bool IsMuted { get; }
        /// <summary> Returns true if this user has marked themselves as deafened. </summary>
        bool IsSelfDeafened { get; }
        /// <summary> Returns true if this user has marked themselves as muted. </summary>
        bool IsSelfMuted { get; }
        /// <summary> Returns true if the guild is temporarily blocking audio to/from this user. </summary>
        bool IsSuppressed { get; }
        /// <summary> Gets the voice channel this user is currently in, if any. </summary>
        IVoiceChannel VoiceChannel { get; }
        /// <summary> Gets the unique identifier for this user's voice session. </summary>
        string VoiceSessionId { get; }
    }
}
