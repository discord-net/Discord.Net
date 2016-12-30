using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Audio
{
    internal class BufferedAudioTarget : IAudioTarget, IDisposable
    {
        private static readonly byte[] _silencePacket = new byte[] { 0xF8, 0xFF, 0xFE };

        private double _ticksPerFrame;
        private Task _task;
        private DiscordVoiceAPIClient _client;
        private CancellationTokenSource _cancelTokenSource;
        private ConcurrentQueue<byte[]> _queue;

        internal BufferedAudioTarget(DiscordVoiceAPIClient client, int samplesPerFrame, CancellationToken cancelToken)
        {
            _client = client;
            double milliseconds = samplesPerFrame / 48.0;
            double ticksPerFrame = Stopwatch.Frequency / 1000.0 * milliseconds;

            _cancelTokenSource = new CancellationTokenSource();
            cancelToken = CancellationTokenSource.CreateLinkedTokenSource(_cancelTokenSource.Token, cancelToken).Token;
            _queue = new ConcurrentQueue<byte[]>(); //TODO: We need a better queue

            _task = Run(ticksPerFrame, cancelToken);
        }

        private Task Run(double ticksPerFrame, CancellationToken cancelToken)
        {
            return Task.Run(async () =>
            {
                var stopwatch = Stopwatch.StartNew();
                long lastTick = stopwatch.ElapsedTicks;
                double ticksPerMilli = Stopwatch.Frequency / 1000.0;
                while (!cancelToken.IsCancellationRequested)
                {
                    long thisTick = stopwatch.ElapsedTicks;
                    double remaining = ticksPerFrame - (thisTick - lastTick);
                    if (remaining <= 0)
                    {
                        byte[] buffer;
                        if (_queue.TryDequeue(out buffer))
                            await _client.SendAsync(buffer, buffer.Length).ConfigureAwait(false);
                        else
                            await _client.SendAsync(_silencePacket, _silencePacket.Length).ConfigureAwait(false);
                        lastTick = thisTick;
                    }
                    else if (remaining > 1)
                    {
                        int millis = (int)Math.Floor(remaining / ticksPerMilli);
                        await Task.Delay(millis).ConfigureAwait(false);
                    }
                }
            });
        }

        public Task SendAsync(byte[] buffer, int count)
        {
            byte[] newBuffer = new byte[count];
            Buffer.BlockCopy(buffer, 0, newBuffer, 0, count);
            _queue.Enqueue(newBuffer);
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
