using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio.Streams
{
    ///<summary> Converts Opus to PCM </summary>
    public class OpusDecodeStream : AudioOutStream
    {
        public const int SampleRate = OpusEncodeStream.SampleRate;

        private readonly AudioStream _next;
        private readonly byte[] _buffer;
        private readonly OpusDecoder _decoder;

        public OpusDecodeStream(AudioStream next, int channels = OpusConverter.MaxChannels, int bufferSize = 5760 * 4)
        {
            _next = next;
            _buffer = new byte[bufferSize];
            _decoder = new OpusDecoder(SampleRate, channels);
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            count = _decoder.DecodeFrame(buffer, offset, count, _buffer, 0);
            await _next.WriteAsync(_buffer, 0, count, cancellationToken).ConfigureAwait(false);
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
            base.Dispose(disposing);

            if (disposing)
                _decoder.Dispose();
        }
    }
}
