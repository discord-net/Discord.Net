using Discord.Rest;
using System;
using System.Collections.Concurrent;
#if DEBUG_LIMITS
using System.Diagnostics;
#endif
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Queue
{
    internal class RequestQueue : IDisposable
    {
        public event Func<string, RateLimitInfo?, Task> RateLimitTriggered;

        private readonly ConcurrentDictionary<string, RequestBucket> _buckets;
        private readonly SemaphoreSlim _tokenLock;
        private readonly CancellationTokenSource _cancelTokenSource; //Dispose token
        private CancellationTokenSource _clearToken;
        private CancellationToken _parentToken;
        private CancellationTokenSource _requestCancelTokenSource;
        private CancellationToken _requestCancelToken; //Parent token + Clear token
        private DateTimeOffset _waitUntil;

        private readonly Semaphore _masterIdentifySemaphore;
        private readonly Semaphore _identifySemaphore;
        private readonly int _identifySemaphoreMaxConcurrency;

        private Task _cleanupTask;

        public RequestQueue()
        {
            _tokenLock = new SemaphoreSlim(1, 1);

            _clearToken = new CancellationTokenSource();
            _cancelTokenSource = new CancellationTokenSource();
            _requestCancelToken = CancellationToken.None;
            _parentToken = CancellationToken.None;

            _buckets = new ConcurrentDictionary<string, RequestBucket>();

            _cleanupTask = RunCleanup();
        }

        public RequestQueue(string masterIdentifySemaphoreName, string slaveIdentifySemaphoreName, int slaveIdentifySemaphoreMaxConcurrency)
            : this ()
        {
            _masterIdentifySemaphore = new Semaphore(1, 1, masterIdentifySemaphoreName);
            _identifySemaphore = new Semaphore(0, GatewayBucket.Get(GatewayBucketType.Identify).WindowCount, slaveIdentifySemaphoreName);
            _identifySemaphoreMaxConcurrency = slaveIdentifySemaphoreMaxConcurrency;
        }

        public async Task SetCancelTokenAsync(CancellationToken cancelToken)
        {
            await _tokenLock.WaitAsync().ConfigureAwait(false);
            try
            {
                _parentToken = cancelToken;
                _requestCancelTokenSource?.Dispose();
                _requestCancelTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancelToken, _clearToken.Token);
                _requestCancelToken = _requestCancelTokenSource.Token;
            }
            finally { _tokenLock.Release(); }
        }
        public async Task ClearAsync()
        {
            await _tokenLock.WaitAsync().ConfigureAwait(false);
            try
            {
                _clearToken?.Cancel();
                _clearToken?.Dispose();
                _clearToken = new CancellationTokenSource();
                if (_parentToken != null)
                {
                    _requestCancelTokenSource?.Dispose();
                    _requestCancelTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_clearToken.Token, _parentToken);
                    _requestCancelToken = _requestCancelTokenSource.Token;
                }
                else
                    _requestCancelToken = _clearToken.Token;
            }
            finally { _tokenLock.Release(); }
        }

        public async Task<Stream> SendAsync(RestRequest request)
        {
            CancellationTokenSource createdTokenSource = null;
            if (request.Options.CancelToken.CanBeCanceled)
            {
                createdTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_requestCancelToken, request.Options.CancelToken);
                request.Options.CancelToken = createdTokenSource.Token;
            }
            else
                request.Options.CancelToken = _requestCancelToken;

            var bucket = GetOrCreateBucket(request.Options.BucketId, request);
            var result = await bucket.SendAsync(request).ConfigureAwait(false);
            createdTokenSource?.Dispose();
            return result;
        }
        public async Task SendAsync(WebSocketRequest request)
        {
            CancellationTokenSource createdTokenSource = null;
            if (request.Options.CancelToken.CanBeCanceled)
            {
                createdTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_requestCancelToken, request.Options.CancelToken);
                request.Options.CancelToken = createdTokenSource.Token;
            }
            else
                request.Options.CancelToken = _requestCancelToken;

            var bucket = GetOrCreateBucket(request.Options.BucketId, request);
            await bucket.SendAsync(request).ConfigureAwait(false);
            createdTokenSource?.Dispose();
        }

        internal async Task EnterGlobalAsync(int id, RestRequest request)
        {
            int millis = (int)Math.Ceiling((_waitUntil - DateTimeOffset.UtcNow).TotalMilliseconds);
            if (millis > 0)
            {
#if DEBUG_LIMITS
                Debug.WriteLine($"[{id}] Sleeping {millis} ms (Pre-emptive) [Global]");
#endif
                await Task.Delay(millis).ConfigureAwait(false);
            }
        }
        internal void PauseGlobal(RateLimitInfo info)
        {
            _waitUntil = DateTimeOffset.UtcNow.AddMilliseconds(info.RetryAfter.Value + (info.Lag?.TotalMilliseconds ?? 0.0));
        }
        internal async Task EnterGlobalAsync(int id, WebSocketRequest request)
        {
            //If this is a global request (unbucketed), it'll be dealt in EnterAsync
            var requestBucket = GatewayBucket.Get(request.Options.BucketId);
            if (requestBucket.Type == GatewayBucketType.Unbucketed)
                return;

            //Identify is per-account so we won't trigger global until we can actually go for it
            if (requestBucket.Type == GatewayBucketType.Identify)
            {
                if (_masterIdentifySemaphore == null || _identifySemaphore == null)
                    throw new InvalidOperationException("Not a RequestQueue with WebSocket data.");

                bool master;
                while (!(master = _masterIdentifySemaphore.WaitOne(0)) && !_identifySemaphore.WaitOne(0)) //To not block the thread
                    await Task.Delay(100, request.CancelToken);
                if (master && _identifySemaphoreMaxConcurrency > 1)
                    _identifySemaphore.Release(_identifySemaphoreMaxConcurrency - 1);
#if DEBUG_LIMITS
                Debug.WriteLine($"[{id}] Acquired identify ticket");
#endif
            }

            //It's not a global request, so need to remove one from global (per-session)
            var globalBucketType = GatewayBucket.Get(GatewayBucketType.Unbucketed);
            var options = RequestOptions.CreateOrClone(request.Options);
            options.BucketId = globalBucketType.Id;
            var globalRequest = new WebSocketRequest(null, null, false, options);
            var globalBucket = GetOrCreateBucket(globalBucketType.Id, globalRequest);
            await globalBucket.TriggerAsync(id, globalRequest);
        }
        internal void ReleaseIdentifySemaphore(int id)
        {
            if (_masterIdentifySemaphore == null || _identifySemaphore == null)
                throw new InvalidOperationException("Not a RequestQueue with WebSocket data.");

            while (_identifySemaphore.WaitOne(0)) //exhaust all tickets before releasing master
            { }
            _masterIdentifySemaphore.Release();
#if DEBUG_LIMITS
            Debug.WriteLine($"[{id}] Released identify ticket");
#endif
        }

        private RequestBucket GetOrCreateBucket(string id, IRequest request)
        {
            return _buckets.GetOrAdd(id, x => new RequestBucket(this, request, x));
        }
        internal async Task RaiseRateLimitTriggered(string bucketId, RateLimitInfo? info)
        {
            await RateLimitTriggered(bucketId, info).ConfigureAwait(false);
        }

        private async Task RunCleanup()
        {
            try
            {
                while (!_cancelTokenSource.IsCancellationRequested)
                {
                    var now = DateTimeOffset.UtcNow;
                    foreach (var bucket in _buckets.Select(x => x.Value))
                    {
                        if ((now - bucket.LastAttemptAt).TotalMinutes > 1.0)
                            _buckets.TryRemove(bucket.Id, out _);
                    }
                    await Task.Delay(60000, _cancelTokenSource.Token).ConfigureAwait(false); //Runs each minute
                }
            }
            catch (OperationCanceledException) { }
            catch (ObjectDisposedException) { }
        }

        public void Dispose()
        {
            _cancelTokenSource?.Dispose();
            _tokenLock?.Dispose();
            _clearToken?.Dispose();
            _requestCancelTokenSource?.Dispose();
        }
    }
}
