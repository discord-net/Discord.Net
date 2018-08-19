using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

#if DEBUG_LIMITS
using System.Diagnostics;
#endif

namespace Discord.Net.Queue
{
    internal class RequestQueue : IDisposable
    {
        private readonly ConcurrentDictionary<string, RequestBucket> _buckets;
        private readonly SemaphoreSlim _tokenLock;
        private readonly CancellationTokenSource _cancelToken; //Dispose token

        private Task _cleanupTask;
        private CancellationTokenSource _clearToken;
        private CancellationToken _parentToken;
        private CancellationToken _requestCancelToken; //Parent token + Clear token
        private DateTimeOffset _waitUntil;

        public RequestQueue()
        {
            _tokenLock = new SemaphoreSlim(1, 1);

            _clearToken = new CancellationTokenSource();
            _cancelToken = new CancellationTokenSource();
            _requestCancelToken = CancellationToken.None;
            _parentToken = CancellationToken.None;

            _buckets = new ConcurrentDictionary<string, RequestBucket>();

            _cleanupTask = RunCleanup();
        }

        public void Dispose() => _cancelToken.Dispose();

        public event Func<string, RateLimitInfo?, Task> RateLimitTriggered;

        public async Task SetCancelTokenAsync(CancellationToken cancelToken)
        {
            await _tokenLock.WaitAsync().ConfigureAwait(false);
            try
            {
                _parentToken = cancelToken;
                _requestCancelToken = CancellationTokenSource.CreateLinkedTokenSource(cancelToken, _clearToken.Token)
                    .Token;
            }
            finally
            {
                _tokenLock.Release();
            }
        }

        public async Task ClearAsync()
        {
            await _tokenLock.WaitAsync().ConfigureAwait(false);
            try
            {
                _clearToken?.Cancel();
                _clearToken = new CancellationTokenSource();
                _requestCancelToken = CancellationTokenSource
                    .CreateLinkedTokenSource(_clearToken.Token, _parentToken).Token;
            }
            finally
            {
                _tokenLock.Release();
            }
        }

        public async Task<Stream> SendAsync(RestRequest request)
        {
            if (request.Options.CancelToken.CanBeCanceled)
                request.Options.CancelToken = CancellationTokenSource
                    .CreateLinkedTokenSource(_requestCancelToken, request.Options.CancelToken).Token;
            else
                request.Options.CancelToken = _requestCancelToken;

            var bucket = GetOrCreateBucket(request.Options.BucketId, request);
            return await bucket.SendAsync(request).ConfigureAwait(false);
        }

        public async Task SendAsync(WebSocketRequest request)
        {
            //TODO: Re-impl websocket buckets
            request.CancelToken = _requestCancelToken;
            await request.SendAsync().ConfigureAwait(false);
        }

        internal async Task EnterGlobalAsync(int id, RestRequest request)
        {
            var millis = (int)Math.Ceiling((_waitUntil - DateTimeOffset.UtcNow).TotalMilliseconds);
            if (millis > 0)
            {
#if DEBUG_LIMITS
                Debug.WriteLine($"[{id}] Sleeping {millis} ms (Pre-emptive) [Global]");
#endif
                await Task.Delay(millis).ConfigureAwait(false);
            }
        }

        internal void PauseGlobal(RateLimitInfo info) => _waitUntil =
            DateTimeOffset.UtcNow.AddMilliseconds(info.RetryAfter.Value + (info.Lag?.TotalMilliseconds ?? 0.0));

        private RequestBucket GetOrCreateBucket(string id, RestRequest request) =>
            _buckets.GetOrAdd(id, x => new RequestBucket(this, request, x));

        internal async Task RaiseRateLimitTriggered(string bucketId, RateLimitInfo? info) =>
            await RateLimitTriggered(bucketId, info).ConfigureAwait(false);

        private async Task RunCleanup()
        {
            try
            {
                while (!_cancelToken.IsCancellationRequested)
                {
                    var now = DateTimeOffset.UtcNow;
                    foreach (var bucket in _buckets.Select(x => x.Value))
                        if ((now - bucket.LastAttemptAt).TotalMinutes > 1.0)
                            _buckets.TryRemove(bucket.Id, out var ignored);
                    await Task.Delay(60000, _cancelToken.Token); //Runs each minute
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
        }
    }
}
