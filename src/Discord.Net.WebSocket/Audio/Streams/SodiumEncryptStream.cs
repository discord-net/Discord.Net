using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio.Streams
{
    ///<summary> Encrypts an RTP frame using libsodium </summary>
    public class SodiumEncryptStream : AudioOutStream
    {
        private readonly AudioOutStream _next;
        private readonly byte[] _nonce, _secretKey;

        //protected readonly byte[] _buffer;

        public SodiumEncryptStream(AudioOutStream next, byte[] secretKey/*, int bufferSize = 4000*/)
        {
            _next = next;
            _secretKey = secretKey;
            //_buffer = new byte[bufferSize]; //TODO: Can Sodium do an in-place encrypt?
            _nonce = new byte[24];
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancelToken)
        {
            cancelToken.ThrowIfCancellationRequested();

            Buffer.BlockCopy(buffer, offset, _nonce, 0, 12); //Copy nonce from RTP header
            count = SecretBox.Encrypt(buffer, offset + 12, count - 12, buffer, 12, _nonce, _secretKey);
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
