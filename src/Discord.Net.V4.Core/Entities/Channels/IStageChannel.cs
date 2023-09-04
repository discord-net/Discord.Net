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

    /// <summary>
    ///     Starts the stage, creating a stage instance.
    /// </summary>
    /// <param name="topic">The topic for the stage/</param>
    /// <param name="sendStartNotification">
    ///     Notify @everyone that a Stage instance has started, requires <see cref="GuildPermission.MentionEveryone"/>.
    /// </param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <param name="token">A <see cref="CancellationToken"/> used to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A task that represents the asynchronous start operation.
    /// </returns>
    Task<IStageInstance> StartStageAsync(
        string topic, bool sendStartNotification = false, RequestOptions? options = null, CancellationToken token = default
    );

    /// <summary>
    ///     Stops the stage, deleting the stage instance.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <param name="token">A <see cref="CancellationToken"/> used to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A task that represents the asynchronous stop operation.
    /// </returns>
    Task StopStageAsync(RequestOptions? options = null, CancellationToken token = default);

    /// <summary>
    ///     Indicates that the bot would like to speak within a stage channel.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <param name="token">A <see cref="CancellationToken"/> used to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A task that represents the asynchronous request to speak operation.
    /// </returns>
    Task RequestToSpeakAsync(RequestOptions? options = null, CancellationToken token = default);

    /// <summary>
    ///     Makes the current user become a speaker within a stage.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <param name="token">A <see cref="CancellationToken"/> used to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A task that represents the asynchronous speaker modify operation.
    /// </returns>
    Task BecomeSpeakerAsync(RequestOptions? options = null, CancellationToken token = default);

    /// <summary>
    ///     Makes the current user a listener.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <param name="token">A <see cref="CancellationToken"/> used to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A task that represents the asynchronous stop operation.
    /// </returns>
    Task StopSpeakingAsync(RequestOptions? options = null, CancellationToken token = default);

    /// <summary>
    ///     Makes a user a speaker within a stage.
    /// </summary>
    /// <param name="userId">The user to make the speaker.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <param name="token">A <see cref="CancellationToken"/> used to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A task that represents the asynchronous move operation.
    /// </returns>
    Task MoveToSpeakerAsync(ulong userId, RequestOptions? options = null, CancellationToken token = default);

    /// <summary>
    ///     Removes a user from speaking.
    /// </summary>
    /// <param name="userId">The user to remove from speaking.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <param name="token">A <see cref="CancellationToken"/> used to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A task that represents the asynchronous remove operation.
    /// </returns>
    Task RemoveFromSpeakerAsync(ulong userId, RequestOptions? options = null, CancellationToken token = default);
}
