using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio
{
    internal class OpusEncodeStream : RTPWriteStream
    {
        public const int SampleRate = 48000;
        private int _frameSize;
        private byte[] _partialFrameBuffer;
        private int _partialFramePos;

        private readonly OpusEncoder _encoder;

        internal OpusEncodeStream(IAudioTarget target, byte[] secretKey, int channels, int samplesPerFrame, uint ssrc, int? bitrate = null)
            : base(target, secretKey, samplesPerFrame, ssrc)
        {
            _encoder = new OpusEncoder(SampleRate, channels);
            _frameSize = samplesPerFrame * channels * 2;
            _partialFrameBuffer = new byte[_frameSize];

            _encoder.SetForwardErrorCorrection(true);
            if (bitrate != null)
                _encoder.SetBitrate(bitrate.Value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            WriteAsync(buffer, offset, count, CancellationToken.None).GetAwaiter().GetResult();
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
                    await base.WriteAsync(_buffer, 0, encFrameSize, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    Buffer.BlockCopy(buffer, offset, _partialFrameBuffer, _partialFramePos, count);
                    _partialFramePos += count;
                    break;
                }
            }
        }

        /*public override void Flush()
        {
            FlushAsync(CancellationToken.None).GetAwaiter().GetResult();
        }
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

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
                _encoder.Dispose();
        }
    }
}
