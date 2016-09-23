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
        private readonly RequestQueue _queue;
        private readonly SemaphoreSlim _semaphore;
        private readonly object _pauseLock;
        private int _pauseEndTick;
        private TaskCompletionSource<byte> _resumeNotifier;

        public Bucket Definition { get; }
        public RequestQueueBucket Parent { get; }
        public Task _resetTask { get; }

        public RequestQueueBucket(RequestQueue queue, Bucket definition, RequestQueueBucket parent = null)
        {
            _queue = queue;
            Definition = definition;
            if (definition.WindowCount != 0)
                _semaphore = new SemaphoreSlim(definition.WindowCount, definition.WindowCount);
            Parent = parent;

            _pauseLock = new object();
            _resumeNotifier = new TaskCompletionSource<byte>();
            _resumeNotifier.SetResult(0);
        }

        public async Task<Stream> SendAsync(IQueuedRequest request)
        {
            while (true)
            {
                try
                {
                    return await SendAsyncInternal(request).ConfigureAwait(false);
                }
                catch (HttpRateLimitException ex)
                {
                    //When a 429 occurs, we drop all our locks. 
                    //This is generally safe though since 429s actually occuring should be very rare.
                    RequestQueueBucket bucket;
                    bool success = FindBucket(ex.BucketId, out bucket);

                    await _queue.RaiseRateLimitTriggered(ex.BucketId, success ? bucket.Definition : null, ex.RetryAfterMilliseconds).ConfigureAwait(false);

                    bucket.Pause(ex.RetryAfterMilliseconds);
                }
            }
        }
        private async Task<Stream> SendAsyncInternal(IQueuedRequest request)
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
                            return await Parent.SendAsyncInternal(request).ConfigureAwait(false);

                        //We have all our semaphores, send the request
                        return await request.SendAsync().ConfigureAwait(false);
                    }
                    catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.BadGateway)
                    {
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

        private bool FindBucket(string id, out RequestQueueBucket bucket)
        {
            //Keep going up until we find a bucket with matching id or we're at the topmost bucket
            if (Definition.Id == id)
            {
                bucket = this;
                return true;
            }
            else if (Parent == null)
            {
                bucket = this;
                return false;
            }
            else
                return Parent.FindBucket(id, out bucket);
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
                    QueueResumeAsync(_resumeNotifier, milliseconds);
                }
            }
        }
        private async Task QueueResumeAsync(TaskCompletionSource<byte> resumeNotifier, int millis)
        {
            await Task.Delay(millis).ConfigureAwait(false);
            resumeNotifier.TrySetResultAsync<byte>(0);
        }

        private async Task EnterAsync(int? endTick)
        {
            if (endTick.HasValue)
            {
                int millis = unchecked(endTick.Value - Environment.TickCount);
                if (millis <= 0 || !await _semaphore.WaitAsync(millis).ConfigureAwait(false))
                    throw new TimeoutException();
            }
            else
                await _semaphore.WaitAsync().ConfigureAwait(false);
        }
        private async Task QueueExitAsync()
        {
            await Task.Delay(Definition.WindowSeconds * 1000).ConfigureAwait(false);
            _semaphore.Release();
        }
    }
}
