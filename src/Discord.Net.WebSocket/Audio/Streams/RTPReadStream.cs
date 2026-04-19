using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio.Streams
{
    ///<summary> Reads the payload from an RTP frame </summary>
    public class RTPReadStream : AudioOutStream
    {
        private readonly AudioStream _next;

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;

        public RTPReadStream(AudioStream next, int bufferSize = 4000)
        {
            _next = next;
        }

        /// <exception cref="OperationCanceledException">The token has had cancellation requested.</exception>
        /// <exception cref="ObjectDisposedException">The associated <see cref="T:System.Threading.CancellationTokenSource" /> has been disposed.</exception>
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancelToken)
        {
            cancelToken.ThrowIfCancellationRequested();

            int headerSize = GetHeaderSize(buffer, offset);

            ushort seq = (ushort)((buffer[offset + 2] << 8) |
                (buffer[offset + 3] << 0));

            uint timestamp = (uint)((buffer[offset + 4] << 24) |
                (buffer[offset + 5] << 16) |
                (buffer[offset + 6] << 8) |
                (buffer[offset + 7] << 0));

            // RFC 3550 §5.1: if the P (padding) bit is set in the first RTP
            // header byte, the last octet of the packet is the padding count,
            // which must be stripped from the payload before it is handed off
            // to the next stream (e.g. the DAVE decryptor). Without this,
            // decryption fails with DecryptionFailure on any padded packet —
            // observed in the wild with real Discord clients that pad voice
            // frames to MTU / silence boundaries.
            int paddingBytes = 0;
            if ((buffer[offset] & 0b0010_0000) != 0 && count > 0)
            {
                paddingBytes = buffer[offset + count - 1];
                if (paddingBytes > count - headerSize)
                {
                    paddingBytes = 0; // malformed — don't overshoot into the header
                }
            }

            int payloadLength = count - headerSize - paddingBytes;
            if (payloadLength <= 0)
            {
                // Pure-padding packet (e.g. RTP keepalive / DTX marker with no
                // real payload). Nothing to decode — drop silently rather than
                // invoking the downstream decryptor with an empty buffer,
                // which would spuriously log DecryptionFailure.
                return Task.CompletedTask;
            }

            _next.WriteHeader(seq, timestamp, false);
            return _next.WriteAsync(buffer, offset + headerSize, payloadLength, cancelToken);
        }

        public static bool TryReadSsrc(byte[] buffer, int offset, out uint ssrc)
        {
            ssrc = 0;
            if (buffer.Length - offset < 12)
                return false;

            int version = (buffer[offset + 0] & 0b1100_0000) >> 6;
            if (version != 2)
                return false;
            int type = (buffer[offset + 1] & 0b01111_1111);
            if (type != 120) //Dynamic Discord type
                return false;

            ssrc = (uint)((buffer[offset + 8] << 24) |
                (buffer[offset + 9] << 16) |
                (buffer[offset + 10] << 8) |
                (buffer[offset + 11] << 0));
            return true;
        }

        public static int GetHeaderSize(byte[] buffer, int offset)
        {
            byte headerByte = buffer[offset];
            bool extension = (headerByte & 0b0001_0000) != 0;
            int csics = (headerByte & 0b0000_1111) >> 4;

            if (!extension)
                return 12 + csics * 4;

            int extensionOffset = offset + 12 + (csics * 4);
            int extensionLength =
                (buffer[extensionOffset + 2] << 8) |
                (buffer[extensionOffset + 3]);
            return extensionOffset + 4 + (extensionLength * 4);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _next.Dispose();
            base.Dispose(disposing);
        }
    }
}
