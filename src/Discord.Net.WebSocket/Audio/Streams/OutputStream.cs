using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio.Streams
{
    ///<summary> Wraps an IAudioClient, sending voice data on write. </summary>
    public class OutputStream : AudioOutStream
    {
        private bool _isSpeaking;

        private readonly DiscordVoiceAPIClient _client;
        public OutputStream(IAudioClient client)
            : this((client as AudioClient).ApiClient) { }
        internal OutputStream(DiscordVoiceAPIClient client)
        {
            _client = client;
        }

        public async Task SetSpeakingAsync(bool isSpeaking)
        {
            if (_isSpeaking != isSpeaking)
            {
                await _client.SendSetSpeaking(isSpeaking).ConfigureAwait(false);
                _isSpeaking = isSpeaking;
            }
        }
        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancelToken)
        {
            cancelToken.ThrowIfCancellationRequested();
            await _client.SendAsync(buffer, offset, count).ConfigureAwait(false);
        }
    }
}