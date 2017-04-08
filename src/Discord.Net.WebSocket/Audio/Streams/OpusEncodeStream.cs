using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio.Streams
{
    ///<summary> Converts PCM to Opus </summary>
    public class OpusEncodeStream : AudioOutStream
    {
        public const int SampleRate = 48000;
        
        private readonly AudioStream _next;
        private readonly OpusEncoder _encoder;
        private readonly byte[] _buffer;
        private int _partialFramePos;

        public OpusEncodeStream(AudioStream next, int bitrate, AudioApplication application)
        {
            _next = next;
            _encoder = new OpusEncoder(bitrate, application);
            _buffer = new byte[OpusConverter.FrameBytes];
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            //Assume threadsafe
            while (count > 0)
            {
                if (_partialFramePos == 0 && count >= OpusConverter.FrameBytes)
                {
                    //We have enough data and no partial frames. Pass the buffer directly to the encoder
                    int encFrameSize = _encoder.EncodeFrame(buffer, offset, _buffer, 0);
                    await _next.WriteAsync(_buffer, 0, encFrameSize, cancellationToken).ConfigureAwait(false);

                    offset += OpusConverter.FrameBytes;
                    count -= OpusConverter.FrameBytes;
                }
                else if (_partialFramePos + count >= OpusConverter.FrameBytes)
                {
                    //We have enough data to complete a previous partial frame.
                    int partialSize = OpusConverter.FrameBytes - _partialFramePos;
                    Buffer.BlockCopy(buffer, offset, _buffer, _partialFramePos, partialSize);
                    int encFrameSize = _encoder.EncodeFrame(_buffer, 0, _buffer, 0);
                    await _next.WriteAsync(_buffer, 0, encFrameSize, cancellationToken).ConfigureAwait(false);

                    offset += partialSize;
                    count -= partialSize;
                    _partialFramePos = 0;
                }
                else
                {
                    //Not enough data to build a complete frame, store this part for later
                    Buffer.BlockCopy(buffer, offset, _buffer, _partialFramePos, count);
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
