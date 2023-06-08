using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio.Streams
{
    ///<summary> Converts Opus to PCM </summary>
    public class OpusDecodeStream : AudioOutStream
    {
        public const int SampleRate = OpusEncodeStream.SampleRate;

        private readonly AudioStream _next;
        private readonly OpusDecoder _decoder;
        private readonly byte[] _buffer;
        private bool _nextMissed;
        private bool _hasHeader;

        public OpusDecodeStream(AudioStream next)
        {
            _next = next;
            _buffer = new byte[OpusConverter.FrameBytes];
            _decoder = new OpusDecoder();
        }

        /// <exception cref="InvalidOperationException">Header received with no payload.</exception>
        public override void WriteHeader(ushort seq, uint timestamp, bool missed)
        {
            if (_hasHeader)
                throw new InvalidOperationException("Header received with no payload.");
            _hasHeader = true;

            _nextMissed = missed;
            _next.WriteHeader(seq, timestamp, missed);
        }

        /// <exception cref="InvalidOperationException">Received payload without an RTP header.</exception>
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancelToken)
        {
            if (!_hasHeader)
                throw new InvalidOperationException("Received payload without an RTP header.");
            _hasHeader = false;

            count = !_nextMissed || count > 0
                ? _decoder.DecodeFrame(buffer, offset, count, _buffer, 0, false)
                : _decoder.DecodeFrame(null, 0, 0, _buffer, 0, false);

            return _next.WriteAsync(_buffer, 0, count, cancelToken);
        }

        public override async Task FlushAsync(CancellationToken cancelToken)
        {
            await _next.FlushAsync(cancelToken).ConfigureAwait(false);
        }
        public override async Task ClearAsync(CancellationToken cancelToken)
        {
            await _next.ClearAsync(cancelToken).ConfigureAwait(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _decoder.Dispose();
                _next.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
