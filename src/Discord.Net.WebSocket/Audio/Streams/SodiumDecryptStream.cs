using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio.Streams
{
    ///<summary> Decrypts an RTP frame using libsodium </summary>
    public class SodiumDecryptStream : AudioOutStream
    {
        private readonly AudioOutStream _next;
        private readonly byte[] _buffer, _nonce, _secretKey;

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;

        public SodiumDecryptStream(AudioOutStream next, byte[] secretKey, int bufferSize = 4000)
        {
            _next = next;
            _secretKey = secretKey;
            _buffer = new byte[bufferSize];
            _nonce = new byte[24];
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancelToken)
        {
            cancelToken.ThrowIfCancellationRequested();

            Buffer.BlockCopy(buffer, 0, _nonce, 0, 12); //Copy RTP header to nonce
            count = SecretBox.Decrypt(buffer, offset, count, _buffer, 0, _nonce, _secretKey);

            var newBuffer = new byte[count];
            Buffer.BlockCopy(_buffer, 0, newBuffer, 0, count);
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
