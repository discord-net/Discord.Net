using System.Threading.Tasks;

namespace Discord.Audio
{
    internal interface IAudioTarget
    {
        Task SendAsync(byte[] buffer, int count);
        Task FlushAsync();
    }
}
