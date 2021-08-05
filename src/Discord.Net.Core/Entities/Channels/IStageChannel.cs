using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a generic Stage Channel.
    /// </summary>
    public interface IStageChannel : IVoiceChannel
    {
        /// <summary>
        ///     Gets the topic of the Stage instance.
        /// </summary>
        /// <remarks>
        ///     If the stage isn't live then this property will be set to <see langword="null"/>.
        /// </remarks>
        string Topic { get; }

        /// <summary>
        ///     The <see cref="StagePrivacyLevel"/> of the current stage.
        /// </summary>
        /// <remarks>
        ///     If the stage isn't live then this property will be set to <see langword="null"/>.
        /// </remarks>
        StagePrivacyLevel? PrivacyLevel { get; }

        /// <summary>
        ///     <see langword="true"/> if stage discovery is disabled, otherwise <see langword="false"/>. 
        /// </summary>
        bool? DiscoverableDisabled { get; }

        /// <summary>
        ///     <see langword="true"/> when the stage is live, otherwise <see langword="false"/>.
        /// </summary>
        /// <remarks>
        ///     If the stage isn't live then this property will be set to <see langword="null"/>.
        /// </remarks>
        bool Live { get; }

        /// <summary>
        ///     Starts the stage, creating a stage instance.
        /// </summary>
        /// <param name="topic">The topic for the stage/</param>
        /// <param name="privacyLevel">The privacy level of the stage</param>
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

        Task RequestToSpeak(RequestOptions options = null);
    }
}
