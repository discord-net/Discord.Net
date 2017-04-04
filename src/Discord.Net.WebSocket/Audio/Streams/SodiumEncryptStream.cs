using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio.Streams
{
    ///<summary> Encrypts an RTP frame using libsodium </summary>
    public class SodiumEncryptStream : AudioOutStream
    {
        private readonly AudioClient _client;
        private readonly AudioStream _next;
        private readonly byte[] _nonce;

        public SodiumEncryptStream(AudioStream next, IAudioClient client)
        {
            _next = next;
            _client = (AudioClient)client;
            _nonce = new byte[24];
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancelToken)
        {
            cancelToken.ThrowIfCancellationRequested();

            if (_client.SecretKey == null)
                return;
                
            Buffer.BlockCopy(buffer, offset, _nonce, 0, 12); //Copy nonce from RTP header
            count = SecretBox.Encrypt(buffer, offset + 12, count - 12, buffer, 12, _nonce, _client.SecretKey);
            await _next.WriteAsync(buffer, 0, count + 12, cancelToken).ConfigureAwait(false);
        }

        public override async Task FlushAsync(CancellationToken cancelToken)
        {
            await _next.FlushAsync(cancelToken).ConfigureAwait(false);
        }
        public override async Task ClearAsync(CancellationToken cancelToken)
        {
            await _next.ClearAsync(cancelToken).ConfigureAwait(false);
        }
    }
}
