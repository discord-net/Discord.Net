using System;
using System.Threading.Tasks;

namespace Discord
{
    public interface IVoiceChannel : IGuildChannel, IAudioChannel
    {
        /// <summary> Gets the bitrate, in bits per second, clients in this voice channel are requested to use. </summary>
        int Bitrate { get; }
        /// <summary> Gets the max amount of users allowed to be connected to this channel at one time. </summary>
        int? UserLimit { get; }

        /// <summary> Modifies this voice channel. </summary>
        Task ModifyAsync(Action<VoiceChannelProperties> func, RequestOptions options = null);
    }
}