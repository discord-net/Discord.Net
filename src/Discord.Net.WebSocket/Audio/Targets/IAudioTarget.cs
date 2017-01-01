using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio
{
    internal interface IAudioTarget
    {
        Task SendAsync(byte[] buffer, int count);
        Task FlushAsync(CancellationToken cancelToken);
        Task ClearAsync(CancellationToken cancelToken);
    }
}
