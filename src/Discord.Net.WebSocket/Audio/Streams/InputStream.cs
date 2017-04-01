using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio.Streams
{
    ///<summary> Reads the payload from an RTP frame </summary>
    public class InputStream : AudioInStream
    {
        private const int MaxFrames = 100;

        private ConcurrentQueue<RTPFrame> _frames;
        private SemaphoreSlim _signal;
        private ushort _nextSeq;
        private uint _nextTimestamp;
        private bool _hasHeader;
        private bool _isDisposed;

        public override bool CanRead => !_isDisposed;
        public override bool CanSeek => false;
        public override bool CanWrite => false;

        public InputStream()
        {
            _frames = new ConcurrentQueue<RTPFrame>();
            _signal = new SemaphoreSlim(0, MaxFrames);
        }

        public override async Task<RTPFrame> ReadFrameAsync(CancellationToken cancelToken)
        {
            cancelToken.ThrowIfCancellationRequested();

            RTPFrame frame;
            await _signal.WaitAsync(cancelToken).ConfigureAwait(false);
            _frames.TryDequeue(out frame);
            return frame;
        }
        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancelToken)
        {
            cancelToken.ThrowIfCancellationRequested();

            var frame = await ReadFrameAsync(cancelToken).ConfigureAwait(false);
            if (count < frame.Payload.Length)
                throw new InvalidOperationException("Buffer is too small.");
            Buffer.BlockCopy(frame.Payload, 0, buffer, offset, frame.Payload.Length);
            return frame.Payload.Length;
        }

        public void WriteHeader(ushort seq, uint timestamp)
        {
            if (_hasHeader)
                throw new InvalidOperationException("Header received with no payload");
            _hasHeader = true;
            _nextSeq = seq;
            _nextTimestamp = timestamp;
        }
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancelToken)
        {
            cancelToken.ThrowIfCancellationRequested();

            if (_frames.Count > MaxFrames) //1-2 seconds
            {
                _hasHeader = false;
                return Task.Delay(0); //Buffer overloaded
            }
            if (!_hasHeader)
                throw new InvalidOperationException("Received payload without an RTP header");
            byte[] payload = new byte[count];
            Buffer.BlockCopy(buffer, offset, payload, 0, count);

            _frames.Enqueue(new RTPFrame(
                sequence: _nextSeq,
                timestamp: _nextTimestamp,
                payload: payload
            ));
            _signal.Release();
            _hasHeader = false;
            return Task.Delay(0);
        }

        protected override void Dispose(bool isDisposing)
        {
            _isDisposed = true;
        }
    }
}
