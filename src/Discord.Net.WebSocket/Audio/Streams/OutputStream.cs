using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio.Streams
{
    ///<summary> Wraps an IAudioClient, sending voice data on write. </summary>
    public class OutputStream : AudioOutStream
    {
        private readonly DiscordVoiceAPIClient _client;
        public OutputStream(IAudioClient client)
            : this((client as AudioClient).ApiClient) { }
        internal OutputStream(DiscordVoiceAPIClient client)
        {
            _client = client;
        }

        public override void WriteHeader(ushort seq, uint timestamp, bool missed) { } //Ignore
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancelToken)
        {
            cancelToken.ThrowIfCancellationRequested();
            return _client.SendAsync(buffer, offset, count);
        }
    }
}
