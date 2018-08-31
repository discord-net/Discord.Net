using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio.Streams
{
    /// <summary>
    ///     Encrypts an RTP frame using libsodium.
    /// </summary>
    public class SodiumEncryptStream : AudioOutStream
    {
        private readonly AudioClient _client;
        private readonly AudioStream _next;
        private readonly byte[] _nonce;
        private bool _hasHeader;
        private ushort _nextSeq;
        private uint _nextTimestamp;

        public SodiumEncryptStream(AudioStream next, IAudioClient client)
        {
            _next = next;
            _client = (AudioClient)client;
            _nonce = new byte[24];
        }

        /// <exception cref="InvalidOperationException">Header received with no payload.</exception>
        public override void WriteHeader(ushort seq, uint timestamp, bool missed)
        {
            if (_hasHeader)
                throw new InvalidOperationException("Header received with no payload.");

            _nextSeq = seq;
            _nextTimestamp = timestamp;
            _hasHeader = true;
        }
        /// <exception cref="InvalidOperationException">Received payload without an RTP header.</exception>
        /// <exception cref="OperationCanceledException">The token has had cancellation requested.</exception>
        /// <exception cref="ObjectDisposedException">The associated <see cref="T:System.Threading.CancellationTokenSource" /> has been disposed.</exception>
        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancelToken)
        {
            cancelToken.ThrowIfCancellationRequested();
            if (!_hasHeader)
                throw new InvalidOperationException("Received payload without an RTP header.");
            _hasHeader = false;

            if (_client.SecretKey == null)
                return;
                
            Buffer.BlockCopy(buffer, offset, _nonce, 0, 12); //Copy nonce from RTP header
            count = SecretBox.Encrypt(buffer, offset + 12, count - 12, buffer, 12, _nonce, _client.SecretKey);
            _next.WriteHeader(_nextSeq, _nextTimestamp, false);
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
