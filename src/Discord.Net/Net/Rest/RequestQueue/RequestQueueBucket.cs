using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Rest
{
    internal class RequestQueueBucket
    {
        private readonly RequestQueue _parent;
        private readonly BucketGroup _bucketGroup;
        private readonly int _bucketId;
        private readonly ulong _guildId;
        private readonly ConcurrentQueue<RestRequest> _queue;
        private readonly SemaphoreSlim _lock;
        private Task _resetTask;
        private bool _waitingToProcess, _destroyed; //TODO: Remove _destroyed
        private int _id;

        public int WindowMaxCount { get; }
        public int WindowSeconds { get; }
        public int WindowCount { get; private set; }

        public RequestQueueBucket(RequestQueue parent, GlobalBucket bucket, int windowMaxCount, int windowSeconds)
            : this(parent, windowMaxCount, windowSeconds)
        {
            _bucketGroup = BucketGroup.Global;
            _bucketId = (int)bucket;
            _guildId = 0;
        }
        public RequestQueueBucket(RequestQueue parent, GuildBucket bucket, ulong guildId, int windowMaxCount, int windowSeconds)
            : this(parent, windowMaxCount, windowSeconds)
        {
            _bucketGroup = BucketGroup.Guild;
            _bucketId = (int)bucket;
            _guildId = guildId;
        }
        private RequestQueueBucket(RequestQueue parent, int windowMaxCount, int windowSeconds)
        {
            _parent = parent;
            WindowMaxCount = windowMaxCount;
            WindowSeconds = windowSeconds;
            _queue = new ConcurrentQueue<RestRequest>();
            _lock = new SemaphoreSlim(1, 1);
            _id = new System.Random().Next(0, int.MaxValue);
        }

        public void Queue(RestRequest request)
        {
            if (_destroyed) throw new Exception();
            //Assume this obj's parent is under lock

            _queue.Enqueue(request);
            Debug($"Request queued ({WindowCount}/{WindowMaxCount} + {_queue.Count})");
        }
        public async Task ProcessQueue(bool acquireLock = false)
        {
            //Assume this obj is under lock

            int nextRetry = 1000;

            //If we have another ProcessQueue waiting to run, dont bother with this one
            if (_waitingToProcess) return;
            _waitingToProcess = true;

            if (acquireLock)
                await Lock().ConfigureAwait(false);
            try
            {
                _waitingToProcess = false;
                while (true)
                {
                    RestRequest request;

                    //If we're waiting to reset (due to a rate limit exception, or preemptive check), abort
                    if (WindowCount == WindowMaxCount) return;
                    //Get next request, return if queue is empty
                    if (!_queue.TryPeek(out request)) break;

                    try
                    {
                        Stream stream;
                        if (request.IsMultipart)
                            stream = await _parent.RestClient.Send(request.Method, request.Endpoint, request.MultipartParams).ConfigureAwait(false);
                        else
                            stream = await _parent.RestClient.Send(request.Method, request.Endpoint, request.Json).ConfigureAwait(false);
                        request.Promise.SetResult(stream);
                    }
                    catch (HttpRateLimitException ex) //Preemptive check failed, use Discord's time instead of our own
                    {
                        WindowCount = WindowMaxCount;
                        var task = _resetTask;
                        if (task != null)
                        {
                            Debug($"External rate limit: Extended to {ex.RetryAfterMilliseconds} ms");
                            var retryAfter = DateTime.UtcNow.AddMilliseconds(ex.RetryAfterMilliseconds);
                            await task.ConfigureAwait(false);
                            int millis = (int)Math.Ceiling((DateTime.UtcNow - retryAfter).TotalMilliseconds);
                            _resetTask = ResetAfter(millis);
                        }
                        else
                        {
                            Debug($"External rate limit: Reset in {ex.RetryAfterMilliseconds} ms");
                            _resetTask = ResetAfter(ex.RetryAfterMilliseconds);
                        }
                        return;
                    }
                    catch (HttpException ex)
                    {
                        if (ex.StatusCode == HttpStatusCode.BadGateway) //Gateway unavailable, retry
                        {
                            await Task.Delay(nextRetry).ConfigureAwait(false);
                            nextRetry *= 2;
                            if (nextRetry > 30000)
                                nextRetry = 30000;
                            continue;
                        }
                        else
                        {
                            //We dont need to throw this here, pass the exception via the promise
                            request.Promise.SetException(ex);
                        }
                    }

                    //Request completed or had an error other than 429
                    _queue.TryDequeue(out request);
                    WindowCount++;
                    nextRetry = 1000;
                    Debug($"Request succeeded ({WindowCount}/{WindowMaxCount} + {_queue.Count})");

                    if (WindowCount == 1 && WindowSeconds > 0)
                    {
                        //First request for this window, schedule a reset
                        _resetTask = ResetAfter(WindowSeconds * 1000);
                        Debug($"Internal rate limit: Reset in {WindowSeconds * 1000} ms");
                    }
                }

                //If queue is empty, non-global, and there is no active rate limit, remove this bucket
                if (_resetTask == null && _bucketGroup == BucketGroup.Guild)
                {
                    try
                    {
                        await _parent.Lock().ConfigureAwait(false);
                        if (_queue.IsEmpty) //Double check, in case a request was queued before we got both locks
                        {
                            Debug($"Destroy");
                            _parent.DestroyGuildBucket((GuildBucket)_bucketId, _guildId);
                            _destroyed = true;
                        }
                    }
                    finally
                    {
                        _parent.Unlock();
                    }
                }
            }
            finally
            {
                if (acquireLock)
                    Unlock();
            }
        }
        public void Clear()
        {
            //Assume this obj is under lock
            RestRequest request;

            while (_queue.TryDequeue(out request)) { }
        }

        private async Task ResetAfter(int milliseconds)
        {
            if (milliseconds > 0)
                await Task.Delay(milliseconds).ConfigureAwait(false);
            try
            {
                await Lock().ConfigureAwait(false);

                Debug($"Reset");

                //Reset the current window count and set our state back to normal
                WindowCount = 0;
                _resetTask = null;

                //Wait is over, work through the current queue
                await ProcessQueue().ConfigureAwait(false);
            }
            finally
            {
                Unlock();
            }
        }

        public async Task Lock()
        {
            await _lock.WaitAsync();
        }
        public void Unlock()
        {
            _lock.Release();
        }

        //TODO: Remove
        private void Debug(string text)
        {
            string name;
            switch (_bucketGroup)
            {
                case BucketGroup.Global:
                    name = ((GlobalBucket)_bucketId).ToString();
                    break;
                case BucketGroup.Guild:
                    name = ((GuildBucket)_bucketId).ToString();
                    break;
                default:
                    name = "Unknown";
                    break;
            }
            System.Diagnostics.Debug.WriteLine($"[{name} {_id}] {text}");
        }
    }
}
