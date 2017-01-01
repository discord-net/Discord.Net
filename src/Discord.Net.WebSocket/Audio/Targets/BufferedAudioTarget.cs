using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio
{
    internal class BufferedAudioTarget : IAudioTarget, IDisposable
    {
        private struct Frame
        {
            public Frame(byte[] buffer, int bytes)
            {
                Buffer = buffer;
                Bytes = bytes;
            }

            public readonly byte[] Buffer;
            public readonly int Bytes;
        }

        private static readonly byte[] _silenceFrame = new byte[] { 0xF8, 0xFF, 0xFE };
        
        private Task _task;
        private DiscordVoiceAPIClient _client;
        private CancellationTokenSource _cancelTokenSource;
        private CancellationToken _cancelToken;
        private ConcurrentQueue<Frame> _queuedFrames;
        private ConcurrentQueue<byte[]> _bufferPool;
        private SemaphoreSlim _queueLock;
        private int _ticksPerFrame;

        internal BufferedAudioTarget(DiscordVoiceAPIClient client, int samplesPerFrame, int bufferMillis, CancellationToken cancelToken)
        {
            _client = client;
            _ticksPerFrame = samplesPerFrame / 48;
            int queueLength = (bufferMillis + (_ticksPerFrame - 1)) / _ticksPerFrame; //Round up

            _cancelTokenSource = new CancellationTokenSource();
            _cancelToken = CancellationTokenSource.CreateLinkedTokenSource(_cancelTokenSource.Token, cancelToken).Token;
            _queuedFrames = new ConcurrentQueue<Frame>();
            _bufferPool = new ConcurrentQueue<byte[]>();
            for (int i = 0; i < queueLength; i++)
                _bufferPool.Enqueue(new byte[1275]);
            _queueLock = new SemaphoreSlim(queueLength, queueLength);

            _task = Run();
        }

        private Task Run()
        {
            return Task.Run(async () =>
            {
                try
                {
                    long nextTick = Environment.TickCount;
                    while (!_cancelToken.IsCancellationRequested)
                    {
                        long tick = Environment.TickCount;
                        long dist = nextTick - tick;
                        if (dist <= 0)
                        {
                            Frame frame;
                            if (_queuedFrames.TryDequeue(out frame))
                            {
                                await _client.SendAsync(frame.Buffer, frame.Bytes).ConfigureAwait(false);
                                _bufferPool.Enqueue(frame.Buffer);
                                _queueLock.Release();
                            }
                            else
                                await _client.SendAsync(_silenceFrame, _silenceFrame.Length).ConfigureAwait(false);
                            nextTick += _ticksPerFrame;
                        }
                        else if (dist > 1)
                            await Task.Delay((int)dist).ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException) { }
            });
        }

        public async Task SendAsync(byte[] data, int count)
        {
            await _queueLock.WaitAsync(-1, _cancelToken).ConfigureAwait(false);
            byte[] buffer;
            _bufferPool.TryDequeue(out buffer);
            Buffer.BlockCopy(data, 0, buffer, 0, count);
            _queuedFrames.Enqueue(new Frame(buffer, count));
        }
        
        public async Task FlushAsync(CancellationToken cancelToken)
        {
            while (true)
            {
                cancelToken.ThrowIfCancellationRequested();
                if (_queuedFrames.Count == 0)
                    return;
                await Task.Delay(250, cancelToken).ConfigureAwait(false);
            }
        }
        public Task ClearAsync(CancellationToken cancelToken)
        {
            Frame ignored;
            do
                cancelToken.ThrowIfCancellationRequested();
            while (_queuedFrames.TryDequeue(out ignored));
            return Task.Delay(0);
        }
        protected void Dispose(bool disposing)
        {
            if (disposing)
                _cancelTokenSource.Cancel();
        }
        public void Dispose()
        {
            Dispose(true);
        }
    }
}