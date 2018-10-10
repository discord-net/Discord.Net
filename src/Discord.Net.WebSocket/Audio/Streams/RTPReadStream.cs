using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio.Streams
{
    ///<summary> Reads the payload from an RTP frame </summary>
    public class RTPReadStream : AudioOutStream
    {
        private readonly AudioStream _next;
        private readonly byte[] _buffer, _nonce;

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;

        public RTPReadStream(AudioStream next, int bufferSize = 4000)
        {
            _next = next;
            _buffer = new byte[bufferSize];
            _nonce = new byte[24];
        }

        /// <exception cref="OperationCanceledException">The token has had cancellation requested.</exception>
        /// <exception cref="ObjectDisposedException">The associated <see cref="T:System.Threading.CancellationTokenSource" /> has been disposed.</exception>
        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancelToken)
        {
            cancelToken.ThrowIfCancellationRequested();

            int headerSize = GetHeaderSize(buffer, offset);

            ushort seq =  (ushort)((buffer[offset + 2] << 8) |
                (buffer[offset + 3] << 0));

            uint timestamp = (uint)((buffer[offset + 4] << 24) |
                (buffer[offset + 5] << 16) |
                (buffer[offset + 6] << 8) |
                (buffer[offset + 7] << 0));

            _next.WriteHeader(seq, timestamp, false);
            await _next.WriteAsync(buffer, offset + headerSize, count - headerSize, cancelToken).ConfigureAwait(false);
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
    }
}
