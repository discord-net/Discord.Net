namespace Discord;

[Flags]
public enum VoiceStateFlags : byte
{
    /// <summary>
    ///     Represents no voice states.
    /// </summary>
    None = 0,

    /// <summary>
    ///     The user is deafened by the guild.
    /// </summary>
    Deafened = 1 << 0,

    /// <summary>
    ///     The user is muted by the guild.
    /// </summary>
    Muted = 1 << 1,

    /// <summary>
    ///     The user has marked themselves as deafened.
    /// </summary>
    SelfDeafened = 1 << 2,

    /// <summary>
    ///     The user has marked themselves as muted.
    /// </summary>
    SelfMuted = 1 << 3,

    /// <summary>
    ///     The user is not permitted to speak.
    /// </summary>
    Suppressed = 1 << 4,

    /// <summary>
    ///     This user is streaming in a voice channel.
    /// </summary>
    Streaming = 1 << 5,

    /// <summary>
    ///     The user is videoing in a voice channel.
    /// </summary>
    Videoing = 1 << 6
}
