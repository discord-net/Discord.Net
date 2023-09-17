namespace Discord;

/// <summary>
///     Flags for the <see cref="IActivity.Flags"/> property, that are ORd together.
///     These describe what the activity payload includes.
/// </summary>
[Flags]
public enum ActivityFlags : ushort
{
    /// <summary>
    ///     Indicates that no actions on this activity can be taken.
    /// </summary>
    None = 0,
    
    // no clue what the hell this is?
    Instance = 1 << 0,
    
    /// <summary>
    ///     Indicates that this activity can be joined.
    /// </summary>
    Join = 1 << 1,
    
    /// <summary>
    ///     Indicates that this activity can be spectated.
    /// </summary>
    Spectate = 1 << 2,
    
    /// <summary>
    ///     Indicates that a user may request to join an activity.
    /// </summary>
    JoinRequest = 1 << 3,
    
    /// <summary>
    ///     Indicates that a user can listen along in Spotify.
    /// </summary>
    Sync = 1 << 4,
    
    /// <summary>
    ///     Indicates that a user can play this song.
    /// </summary>
    Play = 1 << 5,
    
    /// <summary>
    ///     Indicates that a user is playing an activity in a voice channel with friends.
    /// </summary>
    PartyPrivacyFriends = 1 << 6,
    
    /// <summary>
    ///     Indicates that a user is playing an activity in a voice channel.
    /// </summary>
    PartyPrivacyVoiceChannel = 1 << 7,
    
    /// <summary>
    ///     Indicates that a user is playing an embedded activity.
    /// </summary>
    Embedded = 1 << 8
}
