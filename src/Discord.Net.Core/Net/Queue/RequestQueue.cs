using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Queue
{
    public class RequestQueue
    {
        public event Func<string, RequestQueueBucket, int?, Task> RateLimitTriggered;
        
        private readonly SemaphoreSlim _lock;
        private readonly ConcurrentDictionary<string, RequestQueueBucket> _buckets;
        private CancellationTokenSource _clearToken;
        private CancellationToken _parentToken;
        private CancellationToken _cancelToken;
        
        public RequestQueue()
        {
            _lock = new SemaphoreSlim(1, 1);

            _clearToken = new CancellationTokenSource();
            _cancelToken = CancellationToken.None;
            _parentToken = CancellationToken.None;

            _buckets = new ConcurrentDictionary<string, RequestQueueBucket>();
        }
        public async Task SetCancelTokenAsync(CancellationToken cancelToken)
        {
            await _lock.WaitAsync().ConfigureAwait(false);
            try
            {
                _parentToken = cancelToken;
                _cancelToken = CancellationTokenSource.CreateLinkedTokenSource(cancelToken, _clearToken.Token).Token;
            }
            finally { _lock.Release(); }
        }

        public async Task<Stream> SendAsync(RestRequest request)
        {
            request.CancelToken = _cancelToken;
            var bucket = GetOrCreateBucket(request.Options.BucketId);
            return await bucket.SendAsync(request).ConfigureAwait(false);
        }
        public async Task<Stream> SendAsync(WebSocketRequest request)
        {
            request.CancelToken = _cancelToken;
            var bucket = GetOrCreateBucket(request.Options.BucketId);
            return await bucket.SendAsync(request).ConfigureAwait(false);
        }
        
        private RequestQueueBucket GetOrCreateBucket(string id)
        {
            return new RequestQueueBucket(this, id, null);
        }

        public void DestroyBucket(string id)
        {
            //Assume this object is locked
            RequestQueueBucket bucket;
            _buckets.TryRemove(id, out bucket);
        }

        public async Task ClearAsync()
        {
            await _lock.WaitAsync().ConfigureAwait(false);
            try
            {
                _clearToken?.Cancel();
                _clearToken = new CancellationTokenSource();
                if (_parentToken != null)
                    _cancelToken = CancellationTokenSource.CreateLinkedTokenSource(_clearToken.Token, _parentToken).Token;
                else
                    _cancelToken = _clearToken.Token;
            }
            finally { _lock.Release(); }
        }

        internal async Task RaiseRateLimitTriggered(string id, RequestQueueBucket bucket, int? millis)
        {
            await RateLimitTriggered(id, bucket, millis).ConfigureAwait(false);
        }
    }
}
