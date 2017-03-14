using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio.Streams
{
    ///<summary> Reads the payload from an RTP frame </summary>
    public class InputStream : AudioInStream
    {
        private ConcurrentQueue<RTPFrame> _frames;
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
        }

        public override Task<RTPFrame?> ReadFrameAsync(CancellationToken cancelToken)
        {
            if (_frames.TryDequeue(out var frame))
                return Task.FromResult<RTPFrame?>(frame);
            return Task.FromResult<RTPFrame?>(null);
        }
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancelToken)
        {
            cancelToken.ThrowIfCancellationRequested();
            
            if (_frames.TryDequeue(out var frame))
            {
                if (count < frame.Payload.Length)
                    throw new InvalidOperationException("Buffer is too small.");
                Buffer.BlockCopy(frame.Payload, 0, buffer, offset, frame.Payload.Length);
                return Task.FromResult(frame.Payload.Length);
            }
            return Task.FromResult(0);
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

            if (_frames.Count > 100) //1-2 seconds
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
            _hasHeader = false;
            return Task.Delay(0);
        }

        protected override void Dispose(bool isDisposing)
        {
            _isDisposed = true;
        }
    }
}
