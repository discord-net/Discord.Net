namespace Discord;

public interface IStageChannel : IStageChannel<IStageChannel>;

/// <summary>
///     Represents a generic Stage Channel.
/// </summary>
public interface IStageChannel<out TChannel> :
    IVoiceChannel<TChannel>
    where TChannel : IStageChannel<TChannel>
{
    /// <summary>
    ///     Gets a stage instance associated with the current stage channel, if any.
    /// </summary>
    ILoadableEntity<IStageInstance> Instance { get; }
}
