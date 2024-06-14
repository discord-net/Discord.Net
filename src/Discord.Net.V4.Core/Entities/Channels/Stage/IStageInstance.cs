using Discord.Models;
using Discord.Rest;

namespace Discord;

public interface IStageInstance : IStageInstance<IStageInstance>;

/// <summary>
///     Represents a live stage instance within a stage channel.
/// </summary>
public interface IStageInstance<TStageInstance> :
    ISnowflakeEntity
    where TStageInstance : IStageInstance<TStageInstance>
{
    /// <summary>
    ///     Gets the topic of the stage.
    /// </summary>
    string Topic { get; }

    /// <summary>
    ///     Gets the stage privacy level.
    /// </summary>
    StagePrivacyLevel PrivacyLevel { get; }

    /// <summary>
    ///     Gets the guild scheduled event tied to this stage instance, if any; otherwise <see langword="null" />.
    /// </summary>
    ILoadableEntity<ulong, IGuildScheduledEvent>? Event { get; }
}
