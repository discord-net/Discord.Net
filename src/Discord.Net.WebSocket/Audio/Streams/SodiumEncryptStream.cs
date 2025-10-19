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
        private const int RtpHeaderSize = 12;
        private const int NonceSize = 24;

        private readonly AudioClient _client;
        private readonly AudioStream _next;
        private readonly byte[] _rtpHeader;
        private readonly byte[] _nonce;
        private bool _hasHeader;
        private ushort _nextSeq;
        private uint _nextTimestamp;
        private uint _nonceCounter;

        public SodiumEncryptStream(AudioStream next, IAudioClient client)
        {
            _next = next;
            _client = (AudioClient)client;
            _rtpHeader = new byte[RtpHeaderSize];
            _nonce = new byte[NonceSize];
            _nonceCounter = 0;
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

            // The first bytes of the nonce are the counter in big-endian.
            byte[] counterBytes = BitConverter.GetBytes(_nonceCounter);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(counterBytes); // big-endian
            Buffer.BlockCopy(counterBytes, offset, _nonce, 0, counterBytes.Length);
            if (++_nonceCounter >= uint.MaxValue)
                _nonceCounter = 0;

            // Encrypt payload
            Buffer.BlockCopy(buffer, offset, _rtpHeader, 0, _rtpHeader.Length);
            int payloadOffset = offset + _rtpHeader.Length;
            int payloadLength = count - offset - _rtpHeader.Length;
            int encryptedLength = SecretBox.Encrypt(
                buffer,
                payloadOffset,
                payloadLength,
                buffer,
                payloadOffset,
                _rtpHeader,
                _nonce,
                _client.SecretKey);

            // Append nonce to encripted payload
            Buffer.BlockCopy(counterBytes, 0, buffer, payloadOffset + encryptedLength, counterBytes.Length);
            int packageLength = _rtpHeader.Length + encryptedLength + counterBytes.Length;

            _next.WriteHeader(_nextSeq, _nextTimestamp, false);
            await _next.WriteAsync(buffer, offset, packageLength, cancelToken).ConfigureAwait(false);
        }

        public override Task FlushAsync(CancellationToken cancelToken)
            => _next.FlushAsync(cancelToken);

        public override Task ClearAsync(CancellationToken cancelToken)
            => _next.ClearAsync(cancelToken);

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _next.Dispose();
            base.Dispose(disposing);
        }
    }
}
