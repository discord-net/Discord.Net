using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio.Streams
{
    /// <summary>
    ///     Decrypts an RTP frame using libsodium.
    /// </summary>
    public class SodiumDecryptStream : AudioOutStream
    {
        private const int RtpHeaderSize = 12;
        private const int ExtendedRtpHeaderSize = RtpHeaderSize + 4;
        private const int NonceSize = 24;
        private const int NonceCounterSize = 4;

        private readonly AudioClient _client;
        private readonly AudioStream _next;
        private readonly byte[] _rtpHeader;
        private readonly byte[] _nonce;

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;

        public SodiumDecryptStream(AudioStream next, IAudioClient client)
        {
            _next = next;
            _client = (AudioClient)client;
            _rtpHeader = new byte[ExtendedRtpHeaderSize];
            _nonce = new byte[NonceSize];
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancelToken)
        {
            cancelToken.ThrowIfCancellationRequested();

            if (_client.SecretKey == null)
                return Task.CompletedTask;

            // Extract nonce counter from the end of the payload.
            for (int i = 0; i < NonceCounterSize; i++)
                _nonce[i] = buffer[offset + count - NonceCounterSize + i];

            // Extract RTP header
            bool hasExtendedHeader = (buffer[0] & 0x10) != 0;
            int rtpHeaderSize = hasExtendedHeader ? ExtendedRtpHeaderSize : RtpHeaderSize;
            Buffer.BlockCopy(buffer, offset, _rtpHeader, 0, rtpHeaderSize);

            // Decrypt payload
            int payloadOffset = offset + rtpHeaderSize;
            int payloadLength = count - offset - rtpHeaderSize - NonceCounterSize;
            int decryptedLength = SecretBox.Decrypt(
                buffer,
                payloadOffset,
                payloadLength,
                buffer,
                payloadOffset,
                _rtpHeader,
                rtpHeaderSize,
                _nonce,
                _client.SecretKey);

            int packageLength = rtpHeaderSize + decryptedLength;
            return _next.WriteAsync(buffer, offset, packageLength, cancelToken);
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
