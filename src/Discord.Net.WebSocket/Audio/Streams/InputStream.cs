using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio.Streams
{
    ///<summary> Reads the payload from an RTP frame </summary>
    public class InputStream : AudioInStream
    {
        private const int MaxFrames = 100; //1-2 Seconds

        private readonly AudioClient _client;
        private ConcurrentQueue<RTPFrame> _frames;
        private SemaphoreSlim _signal;
        private ushort _nextSeq;
        private uint _nextTimestamp;
        private bool _nextMissed;
        private bool _hasHeader;
        private bool _isDisposed;

        public override bool CanRead => !_isDisposed;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override int AvailableFrames => _signal.CurrentCount;

        public InputStream(IAudioClient client)
        {
            _client = (AudioClient)client;
            _frames = new ConcurrentQueue<RTPFrame>();
            _signal = new SemaphoreSlim(0, MaxFrames);
        }

        public override bool TryReadFrame(CancellationToken cancelToken, out RTPFrame frame)
        {
            cancelToken.ThrowIfCancellationRequested();

            if (_signal.Wait(0))
            {
                _frames.TryDequeue(out frame);
                return true;
            }
            frame = default(RTPFrame);
            return false;
        }
        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancelToken)
        {
            cancelToken.ThrowIfCancellationRequested();

            var frame = await ReadFrameAsync(cancelToken).ConfigureAwait(false);
            if (count < frame.Payload.Length)
                throw new InvalidOperationException("Buffer is too small.");
            Buffer.BlockCopy(frame.Payload, 0, buffer, offset, frame.Payload.Length);
            _client.RecycleFrame(frame);
            return frame.Payload.Length;
        }
        public override async Task<RTPFrame> ReadFrameAsync(CancellationToken cancelToken)
        {
            cancelToken.ThrowIfCancellationRequested();

            await _signal.WaitAsync(cancelToken).ConfigureAwait(false);
            _frames.TryDequeue(out RTPFrame frame);
            return frame;
        }

        public override void WriteHeader(ushort seq, uint timestamp, bool missed)
        {
            if (_hasHeader)
                throw new InvalidOperationException("Header received with no payload");
            _hasHeader = true;
            _nextSeq = seq;
            _nextTimestamp = timestamp;
            _nextMissed = missed;
        }
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancelToken)
            => WriteAsync(buffer, offset, count, cancelToken, false);
        internal Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancelToken, bool claimBuffer)
        {
            cancelToken.ThrowIfCancellationRequested();

            if (!_hasHeader)
                throw new InvalidOperationException("Received payload without an RTP header");
            _hasHeader = false;

            if (_signal.CurrentCount >= MaxFrames) //1-2 seconds
                return Task.Delay(0); //Buffer overloaded

            _frames.Enqueue(new RTPFrame(
                sequence: _nextSeq,
                timestamp: _nextTimestamp,
                missed: _nextMissed,
                payload: CopyBuffer(buffer, offset, count)
            ));
            _signal.Release();
            return Task.Delay(0);
        }

        protected byte[] CopyBuffer(byte[] buffer, int offset, int count)
        {
            byte[] payload = _client.GetFrameBuffer();
            Buffer.BlockCopy(buffer, offset, payload, 0, count);
            return payload;
        }

        protected override void Dispose(bool isDisposing)
        {
            _isDisposed = true;
        }
    }
}
