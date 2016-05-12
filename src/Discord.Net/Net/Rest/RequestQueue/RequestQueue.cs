using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Rest
{
    public class RequestQueue : IRequestQueue
    {
        private SemaphoreSlim _lock;
        private RequestQueueBucket[] _globalBuckets;
        private Dictionary<ulong, RequestQueueBucket>[] _guildBuckets;

        public IRestClient RestClient { get; }

        public RequestQueue(IRestClient restClient)
        {
            RestClient = restClient;

            _lock = new SemaphoreSlim(1, 1);
            _globalBuckets = new RequestQueueBucket[Enum.GetValues(typeof(GlobalBucket)).Length];
            _guildBuckets = new Dictionary<ulong, RequestQueueBucket>[Enum.GetValues(typeof(GuildBucket)).Length];
        }
        
        internal async Task<Stream> Send(RestRequest request, BucketGroup group, int bucketId, ulong guildId)
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
                case GlobalBucket.DirectMessage: return new RequestQueueBucket(this, bucket, 5, 5);

                default: throw new ArgumentException($"Unknown global bucket: {bucket}", nameof(bucket));
            }
        }
        private RequestQueueBucket CreateBucket(GuildBucket bucket, ulong guildId)
        {
            switch (bucket)
            {
                //Per Guild
                case GuildBucket.SendEditMessage: return new RequestQueueBucket(this, bucket, guildId, 5, 5);
                case GuildBucket.DeleteMessage: return new RequestQueueBucket(this, bucket, guildId, 5, 1);
                case GuildBucket.DeleteMessages: return new RequestQueueBucket(this, bucket, guildId, 1, 1);
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

        public async Task Clear(GlobalBucket type)
        {
            var bucket = _globalBuckets[(int)type];
            if (bucket != null)
            {
                try
                {
                    await bucket.Lock();
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
                        await bucket.Lock();
                        bucket.Clear();
                    }
                    finally { bucket.Unlock(); }
                }
            }
        }
    }
}
