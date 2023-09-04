namespace Discord;

/// <summary>
///     Represents a live stage instance within a stage channel.
/// </summary>
public interface IStageInstance : ISnowflakeEntity, IDeletable, IModifyable<StageInstanceProperties>
{
    /// <summary>
    ///     Gets the guild associated with the stage instance.
    /// </summary>
    IEntitySource<ulong, IGuild> Guild { get; }

    /// <summary>
    ///     Gets the stage channel which this instance is hosted in.
    /// </summary>
    IEntitySource<ulong, IStageChannel> Channel { get; }

    /// <summary>
    ///     Gets the topic of the stage.
    /// </summary>
    string Topic { get; }

    /// <summary>
    ///     Gets the stage privacy level.
    /// </summary>
    StagePrivacyLevel PrivacyLevel { get; }

    /// <summary>
    ///     Gets whether or not this stage is discoverable outside of the current guild.
    /// </summary>
    /// <remarks>
    ///     <b>Deprecated in the Discord API.</b>
    /// </remarks>
    [Obsolete("Deprecated in the Discord API")]
    bool IsDiscoverableDisabled { get; }

    /// <summary>
    ///     Gets the guild scheduled event tied to this stage instance, if any; otherwise <see langword="null"/>.
    /// </summary>
    IEntitySource<ulong, IGuildScheduledEvent>? Event { get; }
}
