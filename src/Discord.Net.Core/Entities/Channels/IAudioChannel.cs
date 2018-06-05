using Discord.Audio;
using System;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a generic audio channel.
    /// </summary>
    public interface IAudioChannel : IChannel
    {
        /// <summary>
        ///     Connects to this audio channel.
        /// </summary>
        Task<IAudioClient> ConnectAsync(bool selfDeaf = false, bool selfMute = false, bool external = false);

        /// <summary> Disconnects from this audio channel. </summary>
        Task DisconnectAsync();
    }
}
