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
        public event Func<string, Bucket, int, Task> RateLimitTriggered;

        private readonly static ImmutableDictionary<GlobalBucket, Bucket> _globalLimits;
        private readonly static ImmutableDictionary<GuildBucket, Bucket> _guildLimits;
        private readonly static ImmutableDictionary<ChannelBucket, Bucket> _channelLimits;
        private readonly SemaphoreSlim _lock;
        private readonly RequestQueueBucket[] _globalBuckets;
        private readonly ConcurrentDictionary<ulong, RequestQueueBucket>[] _guildBuckets;
        private readonly ConcurrentDictionary<ulong, RequestQueueBucket>[] _channelBuckets;
        private CancellationTokenSource _clearToken;
        private CancellationToken _parentToken;
        private CancellationToken _cancelToken;

        static RequestQueue()
        {
            _globalLimits = new Dictionary<GlobalBucket, Bucket>
            {
                //REST
                [GlobalBucket.GeneralRest] = new Bucket(null, "rest", 0, 0, BucketTarget.Both), //No Limit
                //[GlobalBucket.Login] = new BucketDefinition(1, 1),
                [GlobalBucket.DirectMessage] = new Bucket("bot:msg:dm", 5, 5, BucketTarget.Bot),
                [GlobalBucket.SendEditMessage] = new Bucket("bot:msg:global", 50, 10, BucketTarget.Bot),
                //[GlobalBucket.Username] = new Bucket("bot:msg:global", 2, 3600, BucketTarget.Both),

                //Gateway
                [GlobalBucket.GeneralGateway] = new Bucket(null, "gateway", 120, 60, BucketTarget.Both),
                [GlobalBucket.UpdateStatus] = new Bucket(null, "status", 5, 1, BucketTarget.Both, GlobalBucket.GeneralGateway),

                //Rpc
                [GlobalBucket.GeneralRpc] = new Bucket(null, "rpc", 120, 60, BucketTarget.Both)
            }.ToImmutableDictionary();

            _guildLimits = new Dictionary<GuildBucket, Bucket>
            {
                //REST
                [GuildBucket.SendEditMessage] = new Bucket("bot:msg:server", 5, 5, BucketTarget.Bot, GlobalBucket.SendEditMessage),
                [GuildBucket.DeleteMessage] = new Bucket("dmsg", 5, 1, BucketTarget.Bot),
                [GuildBucket.DeleteMessages] = new Bucket("bdmsg", 1, 1, BucketTarget.Bot),
                [GuildBucket.ModifyMember] = new Bucket("guild_member", 10, 10, BucketTarget.Bot),
                [GuildBucket.Nickname] = new Bucket("guild_member_nick", 1, 1, BucketTarget.Bot)
            }.ToImmutableDictionary();

            //Client-Only
            _channelLimits = new Dictionary<ChannelBucket, Bucket>
            {
                //REST
                [ChannelBucket.SendEditMessage] = new Bucket("msg", 10, 10, BucketTarget.Client, GlobalBucket.SendEditMessage),
            }.ToImmutableDictionary();
        }

        public static Bucket GetBucketInfo(GlobalBucket bucket) => _globalLimits[bucket];
        public static Bucket GetBucketInfo(GuildBucket bucket) => _guildLimits[bucket];
        public static Bucket GetBucketInfo(ChannelBucket bucket) => _channelLimits[bucket];

        public RequestQueue()
        {
            _lock = new SemaphoreSlim(1, 1);

            _globalBuckets = new RequestQueueBucket[_globalLimits.Count];
            foreach (var pair in _globalLimits)
            {
                //var target = _globalLimits[pair.Key].Target;
                //if (target == BucketTarget.Both || (target == BucketTarget.Bot && isBot) || (target == BucketTarget.Client && !isBot))
                    _globalBuckets[(int)pair.Key] = CreateBucket(pair.Value);
            }

            _guildBuckets = new ConcurrentDictionary<ulong, RequestQueueBucket>[_guildLimits.Count];
            for (int i = 0; i < _guildLimits.Count; i++)
            {
                //var target = _guildLimits[(GuildBucket)i].Target;
                //if (target == BucketTarget.Both || (target == BucketTarget.Bot && isBot) || (target == BucketTarget.Client && !isBot))
                    _guildBuckets[i] = new ConcurrentDictionary<ulong, RequestQueueBucket>();
            }
            
            _channelBuckets = new ConcurrentDictionary<ulong, RequestQueueBucket>[_channelLimits.Count];
            for (int i = 0; i < _channelLimits.Count; i++)
            {
                //var target = _channelLimits[(GuildBucket)i].Target;
                //if (target == BucketTarget.Both || (target == BucketTarget.Bot && isBot) || (target == BucketTarget.Client && !isBot))
                    _channelBuckets[i] = new ConcurrentDictionary<ulong, RequestQueueBucket>();
            }

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

        internal async Task<Stream> SendAsync(RestRequest request, BucketGroup group, int bucketId, ulong objId)
        {
            request.CancelToken = _cancelToken;
            var bucket = GetBucket(group, bucketId, objId);
            return await bucket.SendAsync(request).ConfigureAwait(false);
        }
        internal async Task<Stream> SendAsync(WebSocketRequest request, BucketGroup group, int bucketId, ulong objId)
        {
            request.CancelToken = _cancelToken;
            var bucket = GetBucket(group, bucketId, objId);
            return await bucket.SendAsync(request).ConfigureAwait(false);
        }
        
        private RequestQueueBucket CreateBucket(Bucket def)
        {
            var parent = def.Parent != null ? GetGlobalBucket(def.Parent.Value) : null;
            return new RequestQueueBucket(this, def, parent);
        }

        public void DestroyGuildBucket(GuildBucket type, ulong guildId)
        {
            //Assume this object is locked
            RequestQueueBucket bucket;
            _guildBuckets[(int)type].TryRemove(guildId, out bucket);
        }
        public void DestroyChannelBucket(ChannelBucket type, ulong channelId)
        {
            //Assume this object is locked
            RequestQueueBucket bucket;
            _channelBuckets[(int)type].TryRemove(channelId, out bucket);
        }

        private RequestQueueBucket GetBucket(BucketGroup group, int bucketId, ulong objId)
        {
            switch (group)
            {
                case BucketGroup.Global:
                    return GetGlobalBucket((GlobalBucket)bucketId);
                case BucketGroup.Guild:
                    return GetGuildBucket((GuildBucket)bucketId, objId);
                case BucketGroup.Channel:
                    return GetChannelBucket((ChannelBucket)bucketId, objId);
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
        private RequestQueueBucket GetChannelBucket(ChannelBucket type, ulong channelId)
        {
            return _channelBuckets[(int)type].GetOrAdd(channelId, _ => CreateBucket(_channelLimits[type]));
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

        internal async Task RaiseRateLimitTriggered(string id, Bucket bucket, int millis)
        {
            await RateLimitTriggered.Invoke(id, bucket, millis).ConfigureAwait(false);
        }
    }
}
