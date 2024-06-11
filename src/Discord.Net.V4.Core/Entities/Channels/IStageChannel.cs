using Discord.EntityRelationships;

namespace Discord;

/// <summary>
///     Represents a generic Stage Channel.
/// </summary>
public interface IStageChannel : IVoiceChannel
{
    /// <summary>
    ///     Gets a stage instance associated with the current stage channel, if any.
    /// </summary>
    ILoadableEntity<IStageInstance> Instance { get; }
}
