using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio.Streams
{
    ///<summary> Converts PCM to Opus </summary>
    public class OpusEncodeStream : AudioOutStream
    {
        public const int SampleRate = 48000;
        
        private readonly AudioOutStream _next;
        private readonly OpusEncoder _encoder;
        private readonly byte[] _buffer;

        private int _frameSize;
        private byte[] _partialFrameBuffer;
        private int _partialFramePos;

        public OpusEncodeStream(AudioOutStream next, int channels, int samplesPerFrame, int bitrate, AudioApplication application, int bufferSize = 4000)
        {
            _next = next;
            _encoder = new OpusEncoder(SampleRate, channels, bitrate, application);
            _frameSize = samplesPerFrame * channels * 2;
            _buffer = new byte[bufferSize];
            _partialFrameBuffer = new byte[_frameSize];
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            //Assume threadsafe
            while (count > 0)
            {
                if (_partialFramePos + count >= _frameSize)
                {
                    int partialSize = _frameSize - _partialFramePos;
                    Buffer.BlockCopy(buffer, offset, _partialFrameBuffer, _partialFramePos, partialSize);
                    offset += partialSize;
                    count -= partialSize;
                    _partialFramePos = 0;

                    int encFrameSize = _encoder.EncodeFrame(_partialFrameBuffer, 0, _frameSize, _buffer, 0);
                    await _next.WriteAsync(_buffer, 0, encFrameSize, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    Buffer.BlockCopy(buffer, offset, _partialFrameBuffer, _partialFramePos, count);
                    _partialFramePos += count;
                    break;
                }
            }
        }

        /*
        public override async Task FlushAsync(CancellationToken cancellationToken)
        {
            try
            {
                int encFrameSize = _encoder.EncodeFrame(_partialFrameBuffer, 0, _partialFramePos, _buffer, 0);
                base.Write(_buffer, 0, encFrameSize);
            }
            catch (Exception) { } //Incomplete frame
            _partialFramePos = 0;
            await base.FlushAsync(cancellationToken).ConfigureAwait(false);
        }*/

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
                _encoder.Dispose();
        }
    }
}
