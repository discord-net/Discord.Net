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

        public Task FlushAsync()
            => Task.Delay(0);
    }
}
