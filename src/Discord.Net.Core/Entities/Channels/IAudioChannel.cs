using System.Threading.Tasks;
using Discord.Audio;

namespace Discord
{
    public interface IAudioChannel : IChannel
    {
        /// <summary> Connects to this audio channel. </summary>
        Task<IAudioClient> ConnectAsync(bool selfDeaf = false, bool selfMute = false, bool external = false);

        /// <summary> Disconnects from this audio channel. </summary>
        Task DisconnectAsync();
    }
}
