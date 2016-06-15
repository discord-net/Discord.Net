using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Queue
{
    public class RequestQueue
    {
        private readonly static ImmutableDictionary<GlobalBucket, BucketDefinition> _globalLimits;
        private readonly static ImmutableDictionary<GuildBucket, BucketDefinition> _guildLimits;
        private readonly SemaphoreSlim _lock;
        private readonly RequestQueueBucket[] _globalBuckets;
        private readonly ConcurrentDictionary<ulong, RequestQueueBucket>[] _guildBuckets;
        private CancellationTokenSource _clearToken;
        private CancellationToken _parentToken;
        private CancellationToken _cancelToken;

        static RequestQueue()
        {
            _globalLimits = new Dictionary<GlobalBucket, BucketDefinition>
            {
                //REST
                [GlobalBucket.GeneralRest] = new BucketDefinition(0, 0), //No Limit
                //[GlobalBucket.Login] = new BucketDefinition(1, 1),
                [GlobalBucket.DirectMessage] = new BucketDefinition(5, 5),
                [GlobalBucket.SendEditMessage] = new BucketDefinition(50, 10),

                //Gateway
                [GlobalBucket.GeneralGateway] = new BucketDefinition(120, 60),
                [GlobalBucket.UpdateStatus] = new BucketDefinition(5, 1, GlobalBucket.GeneralGateway)
            }.ToImmutableDictionary();

            _guildLimits = new Dictionary<GuildBucket, BucketDefinition>
            {
                //REST
                [GuildBucket.SendEditMessage] = new BucketDefinition(5, 5, GlobalBucket.SendEditMessage),
                [GuildBucket.DeleteMessage] = new BucketDefinition(5, 1),
                [GuildBucket.DeleteMessages] = new BucketDefinition(1, 1),
                [GuildBucket.ModifyMember] = new BucketDefinition(10, 10),
                [GuildBucket.Nickname] = new BucketDefinition(1, 1)
            }.ToImmutableDictionary();
        }

        public RequestQueue()
        {
            _lock = new SemaphoreSlim(1, 1);

            _globalBuckets = new RequestQueueBucket[_globalLimits.Count];
            foreach (var pair in _globalLimits)
                _globalBuckets[(int)pair.Key] = CreateBucket(pair.Value);

            _guildBuckets = new ConcurrentDictionary<ulong, RequestQueueBucket>[_guildLimits.Count];
            for (int i = 0; i < _guildLimits.Count; i++)
                _guildBuckets[i] = new ConcurrentDictionary<ulong, RequestQueueBucket>();

            _clearToken = new CancellationTokenSource();
            _cancelToken = CancellationToken.None;
            _parentToken = CancellationToken.None;
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

        internal async Task<Stream> SendAsync(RestRequest request, BucketGroup group, int bucketId, ulong guildId)
        {
            request.CancelToken = _cancelToken;
            var bucket = GetBucket(group, bucketId, guildId);
            return await bucket.SendAsync(request).ConfigureAwait(false);
        }
        internal async Task<Stream> SendAsync(WebSocketRequest request, BucketGroup group, int bucketId, ulong guildId)
        {
            request.CancelToken = _cancelToken;
            var bucket = GetBucket(group, bucketId, guildId);
            return await bucket.SendAsync(request).ConfigureAwait(false);
        }
        
        private RequestQueueBucket CreateBucket(BucketDefinition def)
        {
            var parent = def.Parent != null ? GetGlobalBucket(def.Parent.Value) : null;
            return new RequestQueueBucket(def.WindowCount, def.WindowSeconds * 1000, parent);
        }

        public void DestroyGuildBucket(GuildBucket type, ulong guildId)
        {
            //Assume this object is locked
            RequestQueueBucket bucket;
            _guildBuckets[(int)type].TryRemove(guildId, out bucket);
        }

        private RequestQueueBucket GetBucket(BucketGroup group, int bucketId, ulong guildId)
        {
            switch (group)
            {
                case BucketGroup.Global:
                    return GetGlobalBucket((GlobalBucket)bucketId);
                case BucketGroup.Guild:
                    return GetGuildBucket((GuildBucket)bucketId, guildId);
                default:
                    throw new ArgumentException($"Unknown bucket group: {group}", nameof(group));
            }
        }
        private RequestQueueBucket GetGlobalBucket(GlobalBucket type)
        {
            return _globalBuckets[(int)type];
        }
        private RequestQueueBucket GetGuildBucket(GuildBucket type, ulong guildId)
        {
            return _guildBuckets[(int)type].GetOrAdd(guildId, _ => CreateBucket(_guildLimits[type]));
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
    }
}
