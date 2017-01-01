using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio
{
    internal class DirectAudioTarget : IAudioTarget
    {
        private readonly DiscordVoiceAPIClient _client;
        public DirectAudioTarget(DiscordVoiceAPIClient client)
        {
            _client = client;
        }

        public Task SendAsync(byte[] buffer, int count)
            => _client.SendAsync(buffer, count);

        public Task FlushAsync(CancellationToken cancelToken)
            => Task.Delay(0);
        public Task ClearAsync(CancellationToken cancelToken)
            => Task.Delay(0);
    }
}
