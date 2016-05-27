using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Queue
{
    public class RequestQueue : IRequestQueue
    {
        private readonly SemaphoreSlim _lock;
        private readonly RequestQueueBucket[] _globalBuckets;
        private readonly Dictionary<ulong, RequestQueueBucket>[] _guildBuckets;
        private CancellationTokenSource _clearToken;
        private CancellationToken? _parentToken;
        private CancellationToken _cancelToken;

        public RequestQueue()
        {
            _lock = new SemaphoreSlim(1, 1);
            _globalBuckets = new RequestQueueBucket[Enum.GetValues(typeof(GlobalBucket)).Length];
            _guildBuckets = new Dictionary<ulong, RequestQueueBucket>[Enum.GetValues(typeof(GuildBucket)).Length];
            _clearToken = new CancellationTokenSource();
            _cancelToken = _clearToken.Token;
        }
        internal async Task SetCancelToken(CancellationToken cancelToken)
        {
            await Lock().ConfigureAwait(false);
            try
            {
                _parentToken = cancelToken;
                _cancelToken = CancellationTokenSource.CreateLinkedTokenSource(cancelToken, _clearToken.Token).Token;
            }
            finally { Unlock(); }
        }
        
        internal async Task<Stream> Send(IQueuedRequest request, BucketGroup group, int bucketId, ulong guildId)
        {
            RequestQueueBucket bucket;

            await Lock().ConfigureAwait(false);
            try
            {
                bucket = GetBucket(group, bucketId, guildId);
                bucket.Queue(request);
            }
            finally { Unlock(); }

            //There is a chance the bucket will send this request on its own, but this will simply become a noop then.
            var _ = bucket.ProcessQueue(acquireLock: true).ConfigureAwait(false);

            return await request.Promise.Task.ConfigureAwait(false);
        }

        private RequestQueueBucket CreateBucket(GlobalBucket bucket)
        {
            switch (bucket)
            {
                //Globals
                case GlobalBucket.General: return new RequestQueueBucket(this, bucket, int.MaxValue, 0); //Catch-all
                case GlobalBucket.Login: return new RequestQueueBucket(this, bucket, 1, 1); //TODO: Is this actual logins or token validations too?
                case GlobalBucket.DirectMessage: return new RequestQueueBucket(this, bucket, 5, 5);
                case GlobalBucket.SendEditMessage: return new RequestQueueBucket(this, bucket, 50, 10);
                case GlobalBucket.Gateway: return new RequestQueueBucket(this, bucket, 120, 60);
                case GlobalBucket.UpdateStatus: return new RequestQueueBucket(this, bucket, 5, 1, GlobalBucket.Gateway);

                default: throw new ArgumentException($"Unknown global bucket: {bucket}", nameof(bucket));
            }
        }
        private RequestQueueBucket CreateBucket(GuildBucket bucket, ulong guildId)
        {
            switch (bucket)
            {
                //Per Guild
                case GuildBucket.SendEditMessage: return new RequestQueueBucket(this, bucket, guildId, 5, 5, GlobalBucket.SendEditMessage);
                case GuildBucket.DeleteMessage: return new RequestQueueBucket(this, bucket, guildId, 5, 1);
                case GuildBucket.DeleteMessages: return new RequestQueueBucket(this, bucket, guildId, 1, 1);
                case GuildBucket.ModifyMember: return new RequestQueueBucket(this, bucket, guildId, 10, 10); //TODO: Is this all users or just roles?
                case GuildBucket.Nickname: return new RequestQueueBucket(this, bucket, guildId, 1, 1);

                default: throw new ArgumentException($"Unknown guild bucket: {bucket}", nameof(bucket));
            }
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
            var bucket = _globalBuckets[(int)type];
            if (bucket == null)
            {
                bucket = CreateBucket(type);
                _globalBuckets[(int)type] = bucket;
            }
            return bucket;
        }
        private RequestQueueBucket GetGuildBucket(GuildBucket type, ulong guildId)
        {
            var bucketGroup = _guildBuckets[(int)type];
            if (bucketGroup == null)
            {
                bucketGroup = new Dictionary<ulong, RequestQueueBucket>();
                _guildBuckets[(int)type] = bucketGroup;
            }
            RequestQueueBucket bucket;
            if (!bucketGroup.TryGetValue(guildId, out bucket))
            {
                bucket = CreateBucket(type, guildId);
                bucketGroup[guildId] = bucket;
            }
            return bucket;
        }

        internal void DestroyGlobalBucket(GlobalBucket type)
        {
            //Assume this object is locked

            _globalBuckets[(int)type] = null;
        }
        internal void DestroyGuildBucket(GuildBucket type, ulong guildId)
        {
            //Assume this object is locked

            var bucketGroup = _guildBuckets[(int)type];
            if (bucketGroup != null)
                bucketGroup.Remove(guildId);
        }

        public async Task Lock()
        {
            await _lock.WaitAsync();
        }
        public void Unlock()
        {
            _lock.Release();
        }

        public async Task Clear()
        {
            await Lock().ConfigureAwait(false);
            try
            {
                _clearToken?.Cancel();
                _clearToken = new CancellationTokenSource();
                if (_parentToken != null)
                    _cancelToken = CancellationTokenSource.CreateLinkedTokenSource(_clearToken.Token, _parentToken.Value).Token;
                else
                    _cancelToken = _clearToken.Token;
            }
            finally { Unlock(); }
        }
        public async Task Clear(GlobalBucket type)
        {
            var bucket = _globalBuckets[(int)type];
            if (bucket != null)
            {
                try
                {
                    await bucket.Lock().ConfigureAwait(false);
                    bucket.Clear();
                }
                finally { bucket.Unlock(); }
            }
        }
        public async Task Clear(GuildBucket type, ulong guildId)
        {
            var bucketGroup = _guildBuckets[(int)type];
            if (bucketGroup != null)
            {
                RequestQueueBucket bucket;
                if (bucketGroup.TryGetValue(guildId, out bucket))
                {
                    try
                    {
                        await bucket.Lock().ConfigureAwait(false);
                        bucket.Clear();
                    }
                    finally { bucket.Unlock(); }
                }
            }
        }
    }
}
