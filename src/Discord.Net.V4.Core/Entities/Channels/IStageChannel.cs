using System;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a generic Stage Channel.
    /// </summary>
    public interface IStageChannel : IVoiceChannel
    {
        /// <summary>
        ///     Gets the <see cref="StagePrivacyLevel"/> of the current stage.
        /// </summary>
        /// <remarks>
        ///     If the stage isn't live then this property will be set to <see langword="null"/>.
        /// </remarks>
        StagePrivacyLevel? PrivacyLevel { get; }

        /// <summary>
        ///     Gets whether or not stage discovery is disabled. 
        /// </summary>
        bool? IsDiscoverableDisabled { get; }

        /// <summary>
        ///     Gets whether or not the stage is live.
        /// </summary>
        bool IsLive { get; }

        /// <summary>
        ///     Starts the stage, creating a stage instance.
        /// </summary>
        /// <param name="topic">The topic for the stage/</param>
        /// <param name="privacyLevel">The privacy level of the stage.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous start operation.
        /// </returns>
        Task StartStageAsync(string topic, StagePrivacyLevel privacyLevel = StagePrivacyLevel.GuildOnly, RequestOptions options = null);

        /// <summary>
        ///     Modifies the current stage instance.
        /// </summary>
        /// <param name="func">The properties to modify the stage instance with.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous modify operation.
        /// </returns>
        Task ModifyInstanceAsync(Action<StageInstanceProperties> func, RequestOptions options = null);

        /// <summary>
        ///     Stops the stage, deleting the stage instance.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous stop operation.
        /// </returns>
        Task StopStageAsync(RequestOptions options = null);

        /// <summary>
        ///     Indicates that the bot would like to speak within a stage channel.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous request to speak operation.
        /// </returns>
        Task RequestToSpeakAsync(RequestOptions options = null);

        /// <summary>
        ///     Makes the current user become a speaker within a stage.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous speaker modify operation.
        /// </returns>
        Task BecomeSpeakerAsync(RequestOptions options = null);

        /// <summary>
        ///     Makes the current user a listener.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous stop operation.
        /// </returns>
        Task StopSpeakingAsync(RequestOptions options = null);

        /// <summary>
        ///     Makes a user a speaker within a stage.
        /// </summary>
        /// <param name="user">The user to make the speaker.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous move operation.
        /// </returns>
        Task MoveToSpeakerAsync(IGuildUser user, RequestOptions options = null);

        /// <summary>
        ///     Removes a user from speaking.
        /// </summary>
        /// <param name="user">The user to remove from speaking.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous remove operation.
        /// </returns>
        Task RemoveFromSpeakerAsync(IGuildUser user, RequestOptions options = null);
    }
}
