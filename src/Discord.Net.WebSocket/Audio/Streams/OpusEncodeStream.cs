using System;
using System.Runtime.CompilerServices;
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
        private ushort _seq;
        private uint _timestamp;

        public OpusEncodeStream(AudioStream next, int bitrate, AudioApplication application, int packetLoss)
        {
            _next = next;
            _encoder = new OpusEncoder(bitrate, application, packetLoss);
            _buffer = new byte[OpusConverter.FrameBytes];
        }

        /// <summary>
        ///     Sends silent frames to avoid interpolation errors after breaks in data transmission.
        /// </summary>
        /// <returns>A task representing the asynchronous operation of sending a silent frame.</returns>
        public async Task WriteSilentFramesAsync()
        {
            // https://discord.com/developers/docs/topics/voice-connections#voice-data-interpolation

            byte[] frameBytes = new byte[OpusConverter.FrameBytes];

            // Magic silence numbers.
            frameBytes[0] = 0xF8;
            frameBytes[1] = 0xFF;
            frameBytes[2] = 0xFE;

            // The rest of the array is already zeroes, so no need to fill the rest.

            // Discard any remaining partial frame.
            _partialFramePos = 0;

            const int frameCount = 5; // Discord specifically asks for 5 frames.
            for (int i = 0; i < frameCount; i += 1)
            {
                await WriteAsync(frameBytes, 0, frameBytes.Length).ConfigureAwait(false);
            } 
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancelToken)
        {
            //Assume thread-safe
            while (count > 0)
            {
                if (_partialFramePos == 0 && count >= OpusConverter.FrameBytes)
                {
                    //We have enough data and no partial frames. Pass the buffer directly to the encoder
                    int encFrameSize = _encoder.EncodeFrame(buffer, offset, _buffer, 0);
                    _next.WriteHeader(_seq, _timestamp, false);
                    await _next.WriteAsync(_buffer, 0, encFrameSize, cancelToken).ConfigureAwait(false);

                    offset += OpusConverter.FrameBytes;
                    count -= OpusConverter.FrameBytes;
                    _seq++;
                    _timestamp += OpusConverter.FrameSamplesPerChannel;
                }
                else if (_partialFramePos + count >= OpusConverter.FrameBytes)
                {
                    //We have enough data to complete a previous partial frame.
                    int partialSize = OpusConverter.FrameBytes - _partialFramePos;
                    Buffer.BlockCopy(buffer, offset, _buffer, _partialFramePos, partialSize);
                    int encFrameSize = _encoder.EncodeFrame(_buffer, 0, _buffer, 0);
                    _next.WriteHeader(_seq, _timestamp, false);
                    await _next.WriteAsync(_buffer, 0, encFrameSize, cancelToken).ConfigureAwait(false);

                    offset += partialSize;
                    count -= partialSize;
                    _partialFramePos = 0;
                    _seq++;
                    _timestamp += OpusConverter.FrameSamplesPerChannel;
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

        /* //Opus throws memory errors on bad frames
        public override async Task FlushAsync(CancellationToken cancelToken)
        {
            try
            {
                int encFrameSize = _encoder.EncodeFrame(_partialFrameBuffer, 0, _partialFramePos, _buffer, 0);
                base.Write(_buffer, 0, encFrameSize);
            }
            catch (Exception) { } //Incomplete frame
            _partialFramePos = 0;
            await base.FlushAsync(cancelToken).ConfigureAwait(false);
        }*/

        public override Task FlushAsync(CancellationToken cancelToken)
            => _next.FlushAsync(cancelToken);

        public override Task ClearAsync(CancellationToken cancelToken)
            => _next.ClearAsync(cancelToken);

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _encoder.Dispose();
                _next.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
