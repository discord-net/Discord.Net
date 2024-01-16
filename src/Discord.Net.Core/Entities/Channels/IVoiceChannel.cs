using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a generic voice channel in a guild.
    /// </summary>
    public interface IVoiceChannel : ITextChannel, IAudioChannel
    {
        /// <summary>
        ///     Gets the bit-rate that the clients in this voice channel are requested to use.
        /// </summary>
        /// <returns>
        ///     An <see cref="int"/> representing the bit-rate (bps) that this voice channel defines and requests the
        ///     client(s) to use.
        /// </returns>
        int Bitrate { get; }
        /// <summary>
        ///     Gets the max number of users allowed to be connected to this channel at once.
        /// </summary>
        /// <returns>
        ///     An <see cref="int"/> representing the maximum number of users that are allowed to be connected to this
        ///     channel at once; <see langword="null" /> if a limit is not set.
        /// </returns>
        int? UserLimit { get; }

        /// <summary>
        ///     Gets the video quality mode for this channel.
        /// </summary>
        VideoQualityMode VideoQualityMode { get; }

        /// <summary>
        ///     Modifies this voice channel.
        /// </summary>
        /// <param name="func">The properties to modify the channel with.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous modification operation.
        /// </returns>
        /// <seealso cref="VoiceChannelProperties"/>
        Task ModifyAsync(Action<VoiceChannelProperties> func, RequestOptions options = null);

        /// <summary>
        ///     Sets the voice channel status in the current channel.
        /// </summary>
        /// <param name="status">The string to set as status.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous modification operation.
        /// </returns>
        Task SetStatusAsync(string status, RequestOptions options = null);
    }
}
