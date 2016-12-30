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
        
        private Task _task;
        private DiscordVoiceAPIClient _client;
        private CancellationTokenSource _cancelTokenSource;
        private ConcurrentQueue<byte[]> _queue;

        internal BufferedAudioTarget(DiscordVoiceAPIClient client, int samplesPerFrame, CancellationToken cancelToken)
        {
            _client = client;
            long ticksPerFrame = samplesPerFrame / 48;

            _cancelTokenSource = new CancellationTokenSource();
            cancelToken = CancellationTokenSource.CreateLinkedTokenSource(_cancelTokenSource.Token, cancelToken).Token;
            _queue = new ConcurrentQueue<byte[]>(); //TODO: We need a better queue

            _task = Run(ticksPerFrame, cancelToken);
        }

        private Task Run(long ticksPerFrame, CancellationToken cancelToken)
        {
            return Task.Run(async () =>
            {
                long nextTick = Environment.TickCount;
                while (!cancelToken.IsCancellationRequested)
                {
                    long tick = Environment.TickCount;
                    long dist = nextTick - tick;
                    if (dist <= 0)
                    {
                        byte[] buffer;
                        if (_queue.TryDequeue(out buffer))
                            await _client.SendAsync(buffer, buffer.Length).ConfigureAwait(false);
                        else
                            await _client.SendAsync(_silencePacket, _silencePacket.Length).ConfigureAwait(false);
                        nextTick += ticksPerFrame;
                    }
                    else if (dist > 1)
                        await Task.Delay((int)dist).ConfigureAwait(false);
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

        public async Task FlushAsync()
        {
            while (true)
            {
                if (_queue.Count == 0)
                    return;
                await Task.Delay(250).ConfigureAwait(false);
            }
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