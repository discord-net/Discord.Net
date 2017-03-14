using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio.Streams
{
    ///<summary> Reads the payload from an RTP frame </summary>
    public class RTPReadStream : AudioOutStream
    {
        private readonly InputStream _queue;
        private readonly AudioOutStream _next;
        private readonly byte[] _buffer, _nonce, _secretKey;

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;

        public RTPReadStream(InputStream queue, byte[] secretKey, int bufferSize = 4000)
            : this(queue, null, secretKey, bufferSize) { }
        public RTPReadStream(InputStream queue, AudioOutStream next, byte[] secretKey, int bufferSize = 4000)
        {
            _queue = queue;
            _next = next;
            _secretKey = secretKey;
            _buffer = new byte[bufferSize];
            _nonce = new byte[24];
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancelToken)
        {
            cancelToken.ThrowIfCancellationRequested();

            if (buffer[offset + 0] != 0x80 || buffer[offset + 1] != 0x78)
                return;

            var payload = new byte[count - 12];
            Buffer.BlockCopy(buffer, offset + 12, payload, 0, count - 12);

            ushort seq =  (ushort)((buffer[offset + 2] << 8) |
                (buffer[offset + 3] << 0));

            uint timestamp = (uint)((buffer[offset + 4] << 24) |
                (buffer[offset + 5] << 16) |
                (buffer[offset + 6] << 16) |
                (buffer[offset + 7] << 0));

            _queue.WriteHeader(seq, timestamp);
            await (_next ?? _queue as Stream).WriteAsync(buffer, offset, count, cancelToken).ConfigureAwait(false);
        }

        public static bool TryReadSsrc(byte[] buffer, int offset, out uint ssrc)
        {
            if (buffer.Length - offset < 12)
            {
                ssrc = 0;
                return false;
            }

            ssrc = (uint)((buffer[offset + 8] << 24) |
                (buffer[offset + 9] << 16) |
                (buffer[offset + 10] << 16) |
                (buffer[offset + 11] << 0));
            return true;
        }
    }
}
