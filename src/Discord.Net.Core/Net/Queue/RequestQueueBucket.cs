using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Queue
{
    internal class RequestBucket
    {
        private readonly object _lock;
        private readonly RequestQueue _queue;
        private int _semaphore;
        private DateTimeOffset? _resetTick;

        public string Id { get; private set; }
        public int WindowCount { get; private set; }
        public DateTimeOffset LastAttemptAt { get; private set; }

        public RequestBucket(RequestQueue queue, RestRequest request, string id)
        {
            _queue = queue;
            Id = id;

            _lock = new object();

            if (request.Options.ClientBucketId != null)
                WindowCount = ClientBucket.Get(request.Options.ClientBucketId).WindowCount;
            else
                WindowCount = 1; //Only allow one request until we get a header back
            _semaphore = WindowCount;
            _resetTick = null;
            LastAttemptAt = DateTimeOffset.UtcNow;
        }
        
        static int nextId = 0;
        public async Task<Stream> SendAsync(RestRequest request)
        {
            int id = Interlocked.Increment(ref nextId);
            Debug.WriteLine($"[{id}] Start");
            LastAttemptAt = DateTimeOffset.UtcNow;
            while (true)
            {
                await _queue.EnterGlobalAsync(id, request).ConfigureAwait(false);
                await EnterAsync(id, request).ConfigureAwait(false);

                Debug.WriteLine($"[{id}] Sending...");
                var response = await request.SendAsync().ConfigureAwait(false);
                TimeSpan lag = DateTimeOffset.UtcNow - DateTimeOffset.Parse(response.Headers["Date"]);
                var info = new RateLimitInfo(response.Headers);

                if (response.StatusCode < (HttpStatusCode)200 || response.StatusCode >= (HttpStatusCode)300)
                {
                    switch (response.StatusCode)
                    {
                        case (HttpStatusCode)429:
                            if (info.IsGlobal)
                            {
                                Debug.WriteLine($"[{id}] (!) 429 [Global]");
                                _queue.PauseGlobal(info, lag);
                            }
                            else
                            {
                                Debug.WriteLine($"[{id}] (!) 429");
                                UpdateRateLimit(id, request, info, lag, true);
                            }
                            await _queue.RaiseRateLimitTriggered(Id, info).ConfigureAwait(false);
                            continue; //Retry
                        case HttpStatusCode.BadGateway: //502
                            Debug.WriteLine($"[{id}] (!) 502");
                            continue; //Continue
                        default:
                            string reason = null;
                            if (response.Stream != null)
                            {
                                try
                                {
                                    using (var reader = new StreamReader(response.Stream))
                                    using (var jsonReader = new JsonTextReader(reader))
                                    {
                                        var json = JToken.Load(jsonReader);
                                        reason = json.Value<string>("message");
                                    }
                                }
                                catch { }
                            }
                            throw new HttpException(response.StatusCode, reason);
                    }
                }
                else
                {
                    Debug.WriteLine($"[{id}] Success");
                    UpdateRateLimit(id, request, info, lag, false);
                    Debug.WriteLine($"[{id}] Stop");
                    return response.Stream;
                }
            }
        }

        private async Task EnterAsync(int id, RestRequest request)
        {
            int windowCount;
            DateTimeOffset? resetAt;
            bool isRateLimited = false;

            while (true)
            {
                if (DateTimeOffset.UtcNow > request.TimeoutAt || request.CancelToken.IsCancellationRequested)
                {
                    if (!isRateLimited)
                        throw new TimeoutException();
                    else
                        throw new RateLimitedException();
                }

                lock (_lock)
                {
                    windowCount = WindowCount;
                    resetAt = _resetTick;
                }

                DateTimeOffset? timeoutAt = request.TimeoutAt;
                if (windowCount > 0 && Interlocked.Decrement(ref _semaphore) < 0)
                {
                    if (!isRateLimited)
                    {
                        isRateLimited = true;
                        await _queue.RaiseRateLimitTriggered(Id, null).ConfigureAwait(false);
                    }
                    if (resetAt.HasValue)
                    {
                        if (resetAt > timeoutAt)
                            throw new RateLimitedException();
                        int millis = (int)Math.Ceiling((resetAt.Value - DateTimeOffset.UtcNow).TotalMilliseconds);
                        Debug.WriteLine($"[{id}] Sleeping {millis} ms (Pre-emptive)");
                        if (millis > 0)
                            await Task.Delay(millis, request.CancelToken).ConfigureAwait(false);
                    }
                    else
                    {
                        if ((timeoutAt.Value - DateTimeOffset.UtcNow).TotalMilliseconds < 500.0)
                            throw new RateLimitedException();
                        Debug.WriteLine($"[{id}] Sleeping 500* ms (Pre-emptive)");
                        await Task.Delay(500, request.CancelToken).ConfigureAwait(false);
                    }
                    continue;
                }
                else
                    Debug.WriteLine($"[{id}] Entered Semaphore ({_semaphore}/{WindowCount} remaining)");
                break;
            }
        }

        private void UpdateRateLimit(int id, RestRequest request, RateLimitInfo info, TimeSpan lag, bool is429)
        {
            if (WindowCount == 0)
                return;

            lock (_lock)
            {
                bool hasQueuedReset = _resetTick != null;
                if (info.Limit.HasValue && WindowCount != info.Limit.Value)
                {
                    WindowCount = info.Limit.Value;
                    _semaphore = info.Remaining.Value;
                    Debug.WriteLine($"[{id}] Upgraded Semaphore to {info.Remaining.Value}/{WindowCount}");
                }

                var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                DateTimeOffset? resetTick = null;

                //Using X-RateLimit-Remaining causes a race condition
                /*if (info.Remaining.HasValue)
                {
                    Debug.WriteLine($"[{id}] X-RateLimit-Remaining: " + info.Remaining.Value);
                    _semaphore = info.Remaining.Value;
                }*/
                if (info.RetryAfter.HasValue)
                {
                    //RetryAfter is more accurate than Reset, where available
                    resetTick = DateTimeOffset.UtcNow.AddMilliseconds(info.RetryAfter.Value);
                    Debug.WriteLine($"[{id}] Retry-After: {info.RetryAfter.Value} ({info.RetryAfter.Value} ms)");
                }
                else if (info.Reset.HasValue)
                {
                    resetTick = info.Reset.Value.AddSeconds(/*1.0 +*/ lag.TotalSeconds);
                    int diff = (int)(resetTick.Value - DateTimeOffset.UtcNow).TotalMilliseconds;
                    Debug.WriteLine($"[{id}] X-RateLimit-Reset: {info.Reset.Value.ToUnixTimeSeconds()} ({diff} ms, {lag.TotalMilliseconds} ms lag)");
                }
                else if (request.Options.ClientBucketId != null)
                {
                    resetTick = DateTimeOffset.UtcNow.AddSeconds(ClientBucket.Get(request.Options.ClientBucketId).WindowSeconds);
                    Debug.WriteLine($"[{id}] Client Bucket ({ClientBucket.Get(request.Options.ClientBucketId).WindowSeconds * 1000} ms)");
                }

                if (resetTick == null)
                {
                    WindowCount = 0; //No rate limit info, disable limits on this bucket (should only ever happen with a user token)
                    Debug.WriteLine($"[{id}] Disabled Semaphore");
                    return;
                }

                if (!hasQueuedReset || resetTick > _resetTick)
                {
                    _resetTick = resetTick;
                    LastAttemptAt = resetTick.Value; //Make sure we dont destroy this until after its been reset
                    Debug.WriteLine($"[{id}] Reset in {(int)Math.Ceiling((resetTick - DateTimeOffset.UtcNow).Value.TotalMilliseconds)} ms");

                    if (!hasQueuedReset)
                    {
                        var _ = QueueReset(id, (int)Math.Ceiling((_resetTick.Value - DateTimeOffset.UtcNow).TotalMilliseconds));
                    }
                }
            }
        }
        private async Task QueueReset(int id, int millis)
        {
            while (true)
            {
                if (millis > 0)
                    await Task.Delay(millis).ConfigureAwait(false);
                lock (_lock)
                {
                    millis = (int)Math.Ceiling((_resetTick.Value - DateTimeOffset.UtcNow).TotalMilliseconds);
                    if (millis <= 0) //Make sure we havent gotten a more accurate reset time
                    {
                        Debug.WriteLine($"[{id}] * Reset *");
                        _semaphore = WindowCount;
                        _resetTick = null;
                        return;
                    }
                }
            }
        }
    }
}
