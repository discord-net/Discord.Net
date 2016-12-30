using Discord.Audio;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    public interface ISocketAudioChannel : IAudioChannel
    {
        Task<IAudioClient> ConnectAsync();
    }
}
