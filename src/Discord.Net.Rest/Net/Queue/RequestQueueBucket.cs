using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
#if DEBUG_LIMITS
using System.Diagnostics;
#endif
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

            if (request.Options.IsClientBucket)
                WindowCount = ClientBucket.Get(request.Options.BucketId).WindowCount;
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
#if DEBUG_LIMITS
            Debug.WriteLine($"[{id}] Start");
#endif
            LastAttemptAt = DateTimeOffset.UtcNow;
            while (true)
            {
                await _queue.EnterGlobalAsync(id, request).ConfigureAwait(false);
                await EnterAsync(id, request).ConfigureAwait(false);

#if DEBUG_LIMITS
                Debug.WriteLine($"[{id}] Sending...");
#endif
                RateLimitInfo info = default(RateLimitInfo);
                try
                {
                    var response = await request.SendAsync().ConfigureAwait(false);
                    info = new RateLimitInfo(response.Headers);

                    if (response.StatusCode < (HttpStatusCode)200 || response.StatusCode >= (HttpStatusCode)300)
                    {
                        switch (response.StatusCode)
                        {
                            case (HttpStatusCode)429:
                                if (info.IsGlobal)
                                {
#if DEBUG_LIMITS
                                    Debug.WriteLine($"[{id}] (!) 429 [Global]");
#endif
                                    _queue.PauseGlobal(info);
                                }
                                else
                                {
#if DEBUG_LIMITS
                                    Debug.WriteLine($"[{id}] (!) 429");
#endif
                                    UpdateRateLimit(id, request, info, true);
                                }
                                await _queue.RaiseRateLimitTriggered(Id, info).ConfigureAwait(false);
                                continue; //Retry
                            case HttpStatusCode.BadGateway: //502
#if DEBUG_LIMITS
                                Debug.WriteLine($"[{id}] (!) 502");
#endif
                                if ((request.Options.RetryMode & RetryMode.Retry502) == 0)
                                    throw new HttpException(HttpStatusCode.BadGateway, null);

                                continue; //Retry
                            default:
                                int? code = null;
                                string reason = null;
                                if (response.Stream != null)
                                {
                                    try
                                    {
                                        using (var reader = new StreamReader(response.Stream))
                                        using (var jsonReader = new JsonTextReader(reader))
                                        {
                                            var json = JToken.Load(jsonReader);
                                            try { code = json.Value<int>("code"); } catch { };
                                            try { reason = json.Value<string>("message"); } catch { };
                                        }
                                    }
                                    catch { }
                                }
                                throw new HttpException(response.StatusCode, code, reason);
                        }
                    }
                    else
                    {
#if DEBUG_LIMITS
                        Debug.WriteLine($"[{id}] Success");
#endif
                        return response.Stream;
                    }
                }
                //catch (HttpException) { throw; } //Pass through
                catch (TimeoutException)
                {
#if DEBUG_LIMITS
                    Debug.WriteLine($"[{id}] Timeout");
#endif
                    if ((request.Options.RetryMode & RetryMode.RetryTimeouts) == 0)
                        throw;

                    await Task.Delay(500);
                    continue; //Retry
                }
                /*catch (Exception)
                {
#if DEBUG_LIMITS
                    Debug.WriteLine($"[{id}] Error");
#endif
                    if ((request.Options.RetryMode & RetryMode.RetryErrors) == 0)
                        throw;

                    await Task.Delay(500);
                    continue; //Retry
                }*/
                finally
                {
                    UpdateRateLimit(id, request, info, false);
#if DEBUG_LIMITS
                    Debug.WriteLine($"[{id}] Stop");
#endif
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
                if (DateTimeOffset.UtcNow > request.TimeoutAt || request.Options.CancelToken.IsCancellationRequested)
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

                    if ((request.Options.RetryMode & RetryMode.RetryRatelimit) == 0)
                        throw new RateLimitedException();

                    if (resetAt.HasValue)
                    {
                        if (resetAt > timeoutAt)
                            throw new RateLimitedException();
                        int millis = (int)Math.Ceiling((resetAt.Value - DateTimeOffset.UtcNow).TotalMilliseconds);
#if DEBUG_LIMITS
                        Debug.WriteLine($"[{id}] Sleeping {millis} ms (Pre-emptive)");
#endif
                        if (millis > 0)
                            await Task.Delay(millis, request.Options.CancelToken).ConfigureAwait(false);
                    }
                    else
                    {
                        if ((timeoutAt.Value - DateTimeOffset.UtcNow).TotalMilliseconds < 500.0)
                            throw new RateLimitedException();
#if DEBUG_LIMITS
                        Debug.WriteLine($"[{id}] Sleeping 500* ms (Pre-emptive)");
#endif
                        await Task.Delay(500, request.Options.CancelToken).ConfigureAwait(false);
                    }
                    continue;
                }
#if DEBUG_LIMITS
                else
                    Debug.WriteLine($"[{id}] Entered Semaphore ({_semaphore}/{WindowCount} remaining)");
#endif
                break;
            }
        }

        private void UpdateRateLimit(int id, RestRequest request, RateLimitInfo info, bool is429)
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
#if DEBUG_LIMITS
                    Debug.WriteLine($"[{id}] Upgraded Semaphore to {info.Remaining.Value}/{WindowCount}");
#endif
                }

                var now = DateTimeUtils.ToUnixSeconds(DateTimeOffset.UtcNow);
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
#if DEBUG_LIMITS
                    Debug.WriteLine($"[{id}] Retry-After: {info.RetryAfter.Value} ({info.RetryAfter.Value} ms)");
#endif
                }
                else if (info.Reset.HasValue)
                {
                    resetTick = info.Reset.Value.AddSeconds(info.Lag?.TotalSeconds ?? 1.0);
                    int diff = (int)(resetTick.Value - DateTimeOffset.UtcNow).TotalMilliseconds;
#if DEBUG_LIMITS
                    Debug.WriteLine($"[{id}] X-RateLimit-Reset: {info.Reset.Value.ToUnixTimeSeconds()} ({diff} ms, {info.Lag?.TotalMilliseconds} ms lag)");
#endif
                }
                else if (request.Options.IsClientBucket && request.Options.BucketId != null)
                {
                    resetTick = DateTimeOffset.UtcNow.AddSeconds(ClientBucket.Get(request.Options.BucketId).WindowSeconds);
#if DEBUG_LIMITS
                    Debug.WriteLine($"[{id}] Client Bucket ({ClientBucket.Get(request.Options.BucketId).WindowSeconds * 1000} ms)");
#endif
                }

                if (resetTick == null)
                {
                    WindowCount = 0; //No rate limit info, disable limits on this bucket (should only ever happen with a user token)
#if DEBUG_LIMITS
                    Debug.WriteLine($"[{id}] Disabled Semaphore");
#endif
                    return;
                }

                if (!hasQueuedReset || resetTick > _resetTick)
                {
                    _resetTick = resetTick;
                    LastAttemptAt = resetTick.Value; //Make sure we dont destroy this until after its been reset
#if DEBUG_LIMITS
                    Debug.WriteLine($"[{id}] Reset in {(int)Math.Ceiling((resetTick - DateTimeOffset.UtcNow).Value.TotalMilliseconds)} ms");
#endif

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
#if DEBUG_LIMITS
                        Debug.WriteLine($"[{id}] * Reset *");
#endif
                        _semaphore = WindowCount;
                        _resetTick = null;
                        return;
                    }
                }
            }
        }
    }
}
