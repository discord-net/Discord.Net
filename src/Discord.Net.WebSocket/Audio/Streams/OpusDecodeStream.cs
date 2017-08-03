using System;
using System.Runtime.InteropServices;
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
        private readonly GCHandle _bufferHandle;
        private readonly IntPtr _bufferPtr;

        private bool _nextMissed;
        private bool _hasHeader;
        private bool _isDisposed;

        public OpusDecodeStream(AudioStream next)
        {
            _next = next;
            _buffer = new byte[OpusConverter.FrameBytes];
            _bufferHandle = GCHandle.Alloc(_buffer, GCHandleType.Pinned);
            _bufferPtr = _bufferHandle.AddrOfPinnedObject();
            _decoder = new OpusDecoder();
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing && !_isDisposed)
            {
                _decoder.Dispose();
                _bufferHandle.Free();
                _isDisposed = true;
            }
        }

        public override void WriteHeader(ushort seq, uint timestamp, bool missed)
        {
            if (_hasHeader)
                throw new InvalidOperationException("Header received with no payload");                
            _hasHeader = true;

            _nextMissed = missed;
            _next.WriteHeader(seq, timestamp, missed);
        }
        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancelToken)
        {
            if (!_hasHeader)
                throw new InvalidOperationException("Received payload without an RTP header");
            _hasHeader = false;

            if (!_nextMissed)
            {
                unsafe
                {
                    fixed (byte* inPtr = buffer)
                        count = _decoder.DecodeFrame(inPtr, offset, count, (byte*)_bufferPtr, 0, false);
                }
                await _next.WriteAsync(_buffer, 0, count, cancelToken).ConfigureAwait(false);
            }
            else if (count > 0)
            {
                unsafe
                {
                    fixed(byte* inPtr = buffer)
                        count = _decoder.DecodeFrame(inPtr, offset, count, (byte*)_bufferPtr, 0, true);
                }
                await _next.WriteAsync(_buffer, 0, count, cancelToken).ConfigureAwait(false);
            }
            else
            {
                unsafe
                {
                    count = _decoder.DecodeFrame(null, 0, 0, (byte*)_bufferPtr, 0, true);
                }
                await _next.WriteAsync(_buffer, 0, count, cancelToken).ConfigureAwait(false);
            }
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
