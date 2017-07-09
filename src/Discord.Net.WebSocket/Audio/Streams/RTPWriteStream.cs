using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio.Streams
{
    ///<summary> Wraps data in an RTP frame </summary>
    public class RTPWriteStream : AudioOutStream
    {
        private readonly AudioStream _next;
        private readonly byte[] _header;
        protected readonly byte[] _buffer;
        private uint _ssrc;
        private ushort _nextSeq;
        private uint _nextTimestamp;
        private bool _hasHeader;

        public RTPWriteStream(AudioStream next, uint ssrc, int bufferSize = 4000)
        {
            _next = next;
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

        public override void WriteHeader(ushort seq, uint timestamp, bool missed)
        {
            if (_hasHeader)
                throw new InvalidOperationException("Header received with no payload");
                
            _hasHeader = true;
            _nextSeq = seq;
            _nextTimestamp = timestamp;
        }
        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancelToken)
        {
            cancelToken.ThrowIfCancellationRequested();
            if (!_hasHeader)
                throw new InvalidOperationException("Received payload without an RTP header");
            _hasHeader = false;

            unchecked
            {
                _header[2] = (byte)(_nextSeq >> 8);
                _header[3] = (byte)(_nextSeq >> 0);
                _header[4] = (byte)(_nextTimestamp >> 24);
                _header[5] = (byte)(_nextTimestamp >> 16);
                _header[6] = (byte)(_nextTimestamp >> 8);
                _header[7] = (byte)(_nextTimestamp >> 0);
            }
            Buffer.BlockCopy(_header, 0, _buffer, 0, 12); //Copy RTP header from to the buffer
            Buffer.BlockCopy(buffer, offset, _buffer, 12, count);

            _next.WriteHeader(_nextSeq, _nextTimestamp, false);
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
