#pragma warning disable CS4014
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Queue
{
    internal class RequestQueueBucket
    {
        private readonly int _windowMilliseconds;
        private readonly SemaphoreSlim _semaphore;
        private readonly object _pauseLock;
        private int _pauseEndTick;
        private TaskCompletionSource<byte> _resumeNotifier;

        public RequestQueueBucket Parent { get; }
        public Task _resetTask { get; }

        public RequestQueueBucket(int windowCount, int windowMilliseconds, RequestQueueBucket parent = null)
        {
            if (windowCount != 0)
                _semaphore = new SemaphoreSlim(windowCount, windowCount);
            _pauseLock = new object();
            _resumeNotifier = new TaskCompletionSource<byte>();
            _resumeNotifier.SetResult(0);
            _windowMilliseconds = windowMilliseconds;
            Parent = parent;
        }

        public async Task<Stream> SendAsync(IQueuedRequest request)
        {
            var endTick = request.TimeoutTick;

            //Wait until a spot is open in our bucket
            if (_semaphore != null)
                await EnterAsync(endTick).ConfigureAwait(false);
            try
            {
                while (true)
                {
                    //Get our 429 state
                    Task notifier;
                    int resumeTime;
                    lock (_pauseLock)
                    {
                        notifier = _resumeNotifier.Task;
                        resumeTime = _pauseEndTick;
                    }

                    //Are we paused due to a 429?
                    if (!notifier.IsCompleted)
                    {
                        //If the 429 ends after the maximum time for this request, timeout immediately
                        if (endTick.HasValue && endTick.Value < resumeTime)
                            throw new TimeoutException();

                        //Wait for the 429 to complete
                        await notifier.ConfigureAwait(false);
                    }

                    try
                    {
                        //If there's a parent bucket, pass this request to them
                        if (Parent != null)
                            return await Parent.SendAsync(request).ConfigureAwait(false);

                        //We have all our semaphores, send the request
                        return await request.SendAsync().ConfigureAwait(false);
                    }
                    catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.BadGateway)
                    {
                        continue;
                    }
                    catch (HttpRateLimitException ex)
                    {
                        Pause(ex.RetryAfterMilliseconds);
                        continue;
                    }
                }
            }
            finally
            {
                //Make sure we put this entry back after WindowMilliseconds
                if (_semaphore != null)
                    QueueExitAsync();
            }
        }

        private void Pause(int milliseconds)
        {
            lock (_pauseLock)
            {
                //If we aren't already waiting on a 429's time, create a new notifier task
                if (_resumeNotifier.Task.IsCompleted)
                {
                    _resumeNotifier = new TaskCompletionSource<byte>();
                    _pauseEndTick = unchecked(Environment.TickCount + milliseconds);
                    QueueResumeAsync(milliseconds);
                }
            }
        }
        private async Task QueueResumeAsync(int millis)
        {
            await Task.Delay(millis).ConfigureAwait(false);
            _resumeNotifier.SetResult(0);
        }

        private async Task EnterAsync(int? endTick)
        {
            if (endTick.HasValue)
            {
                int millis = unchecked(Environment.TickCount - endTick.Value);
                if (millis <= 0 || !await _semaphore.WaitAsync(millis).ConfigureAwait(false))
                    throw new TimeoutException();
            }
            else
                await _semaphore.WaitAsync().ConfigureAwait(false);
        }
        private async Task QueueExitAsync()
        {
            await Task.Delay(_windowMilliseconds).ConfigureAwait(false);
            _semaphore.Release();
        }
    }
}
