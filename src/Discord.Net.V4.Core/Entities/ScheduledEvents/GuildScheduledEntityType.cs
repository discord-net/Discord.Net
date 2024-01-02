namespace Discord;

/// <summary>
///     Represents the type of guild scheduled event.
/// </summary>
public enum GuildScheduledEntityType
{
    /// <summary>
    ///     The event doesn't have a set type.
    /// </summary>
    None = 0,

    /// <summary>
    ///     The event is set in a stage channel.
    /// </summary>
    Stage = 1,

    /// <summary>
    ///     The event is set in a voice channel.
    /// </summary>
    Voice = 2,

    /// <summary>
    ///     The event is set for somewhere externally from discord.
    /// </summary>
    External = 3,
}
