using Discord.Models;
using Discord.Rest;
using Discord.Stage;

namespace Discord;

/// <summary>
///     Represents a live stage instance within a stage channel.
/// </summary>
[Refreshable(nameof(Routes.GetStageInstance))]
public partial interface IStageInstance :
    ISnowflakeEntity<IStageInstanceModel>,
    IStageInstanceActor
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
    IGuildScheduledEventActor? Event { get; }
}
