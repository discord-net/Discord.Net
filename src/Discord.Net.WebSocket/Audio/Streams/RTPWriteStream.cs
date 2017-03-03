using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio.Streams
{
    ///<summary> Wraps data in an RTP frame </summary>
    public class RTPWriteStream : AudioOutStream
    {
        private readonly AudioOutStream _next;
        private readonly byte[] _header;
        private int _samplesPerFrame;
        private uint _ssrc, _timestamp = 0;

        protected readonly byte[] _buffer;

        public RTPWriteStream(AudioOutStream next, int samplesPerFrame, uint ssrc, int bufferSize = 4000)
        {
            _next = next;
            _samplesPerFrame = samplesPerFrame;
            _ssrc = ssrc;
            _buffer = new byte[bufferSize];
            _header = new byte[24];
            _header[0] = 0x80;
            _header[1] = 0x78;
            _header[8] = (byte)(_ssrc >> 24);
            _header[9] = (byte)(_ssrc >> 16);
            _header[10] = (byte)(_ssrc >> 8);
            _header[11] = (byte)(_ssrc >> 0);
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            unchecked
            {
                if (_header[3]++ == byte.MaxValue)
                    _header[2]++;

                _timestamp += (uint)_samplesPerFrame;
                _header[4] = (byte)(_timestamp >> 24);
                _header[5] = (byte)(_timestamp >> 16);
                _header[6] = (byte)(_timestamp >> 8);
                _header[7] = (byte)(_timestamp >> 0);
            }
            Buffer.BlockCopy(_header, 0, _buffer, 0, 12); //Copy RTP header from to the buffer
            Buffer.BlockCopy(buffer, offset, _buffer, 12, count);

            await _next.WriteAsync(_buffer, 0, count + 12).ConfigureAwait(false);
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
