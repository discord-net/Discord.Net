/*using Discord.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio.Streams
{
    ///<summary> Wraps another stream with a timed buffer and packet loss detection. </summary>
    public class JitterBuffer : AudioOutStream
    {
        private struct Frame
        {
            public Frame(byte[] buffer, int bytes, ushort sequence, uint timestamp)
            {
                Buffer = buffer;
                Bytes = bytes;
                Sequence = sequence;
                Timestamp = timestamp;
            }

            public readonly byte[] Buffer;
            public readonly int Bytes;
            public readonly ushort Sequence;
            public readonly uint Timestamp;
        }

        private static readonly byte[] _silenceFrame = new byte[0];

        private readonly AudioStream _next;
        private readonly CancellationTokenSource _cancelTokenSource;
        private readonly CancellationToken _cancelToken;
        private readonly Task _task;
        private readonly ConcurrentQueue<Frame> _queuedFrames;
        private readonly ConcurrentQueue<byte[]> _bufferPool;
        private readonly SemaphoreSlim _queueLock;
        private readonly Logger _logger;
        private readonly int _ticksPerFrame, _queueLength;
        private bool _isPreloaded, _hasHeader;

        private ushort _seq, _nextSeq;
        private uint _timestamp, _nextTimestamp;
        private bool _isFirst;

        public JitterBuffer(AudioStream next, int bufferMillis = 60, int maxFrameSize = 1500)
            : this(next, null, bufferMillis, maxFrameSize) { }
        internal JitterBuffer(AudioStream next, Logger logger, int bufferMillis = 60, int maxFrameSize = 1500)
        {
            //maxFrameSize = 1275 was too limiting at 128kbps,2ch,60ms
            _next = next;
            _ticksPerFrame = OpusEncoder.FrameMillis;
            _logger = logger;
            _queueLength = (bufferMillis + (_ticksPerFrame - 1)) / _ticksPerFrame; //Round up

            _cancelTokenSource = new CancellationTokenSource();
            _cancelToken = _cancelTokenSource.Token;
            _queuedFrames = new ConcurrentQueue<Frame>();
            _bufferPool = new ConcurrentQueue<byte[]>();
            for (int i = 0; i < _queueLength; i++)
                _bufferPool.Enqueue(new byte[maxFrameSize]); 
            _queueLock = new SemaphoreSlim(_queueLength, _queueLength);
            
            _isFirst = true;
            _task = Run();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _cancelTokenSource.Cancel();
            base.Dispose(disposing);
        }

        private Task Run()
        {
            return Task.Run(async () =>
            {
                try
                {
                    long nextTick = Environment.TickCount;
                    int silenceFrames = 0;
                    while (!_cancelToken.IsCancellationRequested)
                    {
                        long tick = Environment.TickCount;
                        long dist = nextTick - tick;
                        if (dist > 0)
                        {
                            await Task.Delay((int)dist).ConfigureAwait(false);
                            continue;
                        }
                        nextTick += _ticksPerFrame;
                        if (!_isPreloaded)
                        {
                            await Task.Delay(_ticksPerFrame).ConfigureAwait(false);
                            continue;
                        }

                        if (_queuedFrames.TryPeek(out Frame frame))
                        {
                            silenceFrames = 0;
                            uint distance = (uint)(frame.Timestamp - _timestamp);
                            bool restartSeq = _isFirst;
                            if (!_isFirst)
                            {
                                if (distance > uint.MaxValue - (OpusEncoder.FrameSamplesPerChannel * 50 * 5)) //Negative distances wraps
                                {
                                    _queuedFrames.TryDequeue(out frame);
                                    _bufferPool.Enqueue(frame.Buffer);
                                    _queueLock.Release();
#if DEBUG
                                    var _ = _logger?.DebugAsync($"Dropped frame {_timestamp} ({_queuedFrames.Count} frames buffered)");
#endif
                                    continue; //This is a missed packet less than five seconds old, ignore it
                                }
                            }

                            if (distance == 0 || restartSeq)
                            {
                                //This is the frame we expected
                                _seq = frame.Sequence;
                                _timestamp = frame.Timestamp;
                                _isFirst = false;
                                silenceFrames = 0;

                                _next.WriteHeader(_seq++, _timestamp, false);
                                await _next.WriteAsync(frame.Buffer, 0, frame.Bytes).ConfigureAwait(false);
                                _queuedFrames.TryDequeue(out frame);
                                _bufferPool.Enqueue(frame.Buffer);
                                _queueLock.Release();
#if DEBUG
                                var _ = _logger?.DebugAsync($"Read frame {_timestamp} ({_queuedFrames.Count} frames buffered)");
#endif
                            }
                            else if (distance == OpusEncoder.FrameSamplesPerChannel)
                            {
                                //Missed this frame, but the next queued one might have FEC info
                                _next.WriteHeader(_seq++, _timestamp, true);
                                await _next.WriteAsync(frame.Buffer, 0, frame.Bytes).ConfigureAwait(false);
#if DEBUG
                                var _ = _logger?.DebugAsync($"Recreated Frame {_timestamp} (Next is {frame.Timestamp})  ({_queuedFrames.Count} frames buffered)");
#endif
                            }
                            else
                            {
                                //Missed this frame and we have no FEC data to work with
                                _next.WriteHeader(_seq++, _timestamp, true);
                                await _next.WriteAsync(null, 0, 0).ConfigureAwait(false);
#if DEBUG
                                var _ = _logger?.DebugAsync($"Missed Frame {_timestamp} (Next is {frame.Timestamp}) ({_queuedFrames.Count} frames buffered)");
#endif
                            }
                        }
                        else if (!_isFirst)
                        {
                            //Missed this frame and we have no FEC data to work with
                            _next.WriteHeader(_seq++, _timestamp, true);
                            await _next.WriteAsync(null, 0, 0).ConfigureAwait(false);
                            if (silenceFrames < 5)
                                silenceFrames++;
                            else
                            {
                                _isFirst = true;
                                _isPreloaded = false;
                            }
#if DEBUG
                            var _ = _logger?.DebugAsync($"Missed Frame {_timestamp}  ({_queuedFrames.Count} frames buffered)");
#endif
                        }
                        _timestamp += OpusEncoder.FrameSamplesPerChannel;
                    }
                }
                catch (OperationCanceledException) { }
            });
        }

        public override void WriteHeader(ushort seq, uint timestamp, bool missed)
        {
            if (_hasHeader)
                throw new InvalidOperationException("Header received with no payload");
            _nextSeq = seq;
            _nextTimestamp = timestamp;
            _hasHeader = true;
        }
        public override async Task WriteAsync(byte[] data, int offset, int count, CancellationToken cancelToken)
        {
            if (cancelToken.CanBeCanceled)
                cancelToken = CancellationTokenSource.CreateLinkedTokenSource(cancelToken, _cancelToken).Token;
            else
                cancelToken = _cancelToken;

            if (!_hasHeader)
                throw new InvalidOperationException("Received payload without an RTP header");
            _hasHeader = false;
            
            uint distance = (uint)(_nextTimestamp - _timestamp);
            if (!_isFirst && (distance == 0 || distance > OpusEncoder.FrameSamplesPerChannel * 50 * 5)) //Negative distances wraps
            {
#if DEBUG
                var _ = _logger?.DebugAsync($"Frame {_nextTimestamp} was {distance} samples off. Ignoring.");
#endif
                return; //This is an old frame, ignore
            }

            if (!await _queueLock.WaitAsync(0).ConfigureAwait(false))
            {
#if DEBUG
                var _ = _logger?.DebugAsync($"Buffer overflow");
#endif
                return;
            }
            _bufferPool.TryDequeue(out byte[] buffer);

            Buffer.BlockCopy(data, offset, buffer, 0, count);
#if DEBUG
            {
                var _ = _logger?.DebugAsync($"Queued Frame {_nextTimestamp}.");
            }
#endif
            _queuedFrames.Enqueue(new Frame(buffer, count, _nextSeq, _nextTimestamp));
            if (!_isPreloaded && _queuedFrames.Count >= _queueLength)
            {
#if DEBUG
                var _ = _logger?.DebugAsync($"Preloaded");
#endif
                _isPreloaded = true;
            }
        }

        public override async Task FlushAsync(CancellationToken cancelToken)
        {
            while (true)
            {
                cancelToken.ThrowIfCancellationRequested();
                if (_queuedFrames.Count == 0)
                    return;
                await Task.Delay(250, cancelToken).ConfigureAwait(false);
            }
        }
        public override Task ClearAsync(CancellationToken cancelToken)
        {
            do
                cancelToken.ThrowIfCancellationRequested();
            while (_queuedFrames.TryDequeue(out Frame ignored));
            return Task.Delay(0);
        }
    }
}*/