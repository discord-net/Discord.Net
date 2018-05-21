using System;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a voice channel in a guild.
    /// </summary>
    public interface IVoiceChannel : IGuildChannel, IAudioChannel
    {
        /// <summary>
        ///     Gets the bitrate, in bits per second, clients in this voice channel are requested to use.
        /// </summary>
        int Bitrate { get; }
        /// <summary>
        ///     Gets the max amount of users allowed to be connected to this channel at one time, or
        ///     <c>null</c> if none is set.
        /// </summary>
        int? UserLimit { get; }

        /// <summary>
        ///     Modifies this voice channel.
        /// </summary>
        /// <param name="func">The properties to modify the channel with.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        Task ModifyAsync(Action<VoiceChannelProperties> func, RequestOptions options = null);
    }
}
