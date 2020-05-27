using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Discord.Net
{
    public class BucketId : IEquatable<BucketId>
    {
        public string HttpMethod { get; }
        public string Endpoint { get; }
        public IOrderedEnumerable<KeyValuePair<string, string>> MajorParams { get; }
        public string BucketHash { get; }

        public bool IsHashBucket { get => BucketHash != null; }

        private BucketId(string httpMethod, string endpoint, IEnumerable<KeyValuePair<string, string>> majorParams, string bucketHash)
        {
            HttpMethod = httpMethod;
            Endpoint = endpoint;
            MajorParams = majorParams.OrderBy(x => x.Key);
            BucketHash = bucketHash;
        }

        public static BucketId Create(string httpMethod, string endpoint, Dictionary<string, string> majorParams)
        {
            Preconditions.NotNullOrWhitespace(httpMethod, nameof(httpMethod));
            Preconditions.NotNullOrWhitespace(endpoint, nameof(endpoint));
            majorParams ??= new Dictionary<string, string>();
            return new BucketId(httpMethod, endpoint, majorParams, null);
        }

        public static BucketId Create(string hash, BucketId oldBucket)
        {
            Preconditions.NotNullOrWhitespace(hash, nameof(hash));
            Preconditions.NotNull(oldBucket, nameof(oldBucket));
            return new BucketId(null, null, oldBucket.MajorParams, hash);
        }

        public string GetBucketHash()
            => IsHashBucket ? $"{BucketHash}:{string.Join("/", MajorParams.Select(x => x.Value))}" : null;

        public string GetUniqueEndpoint()
            => HttpMethod != null ? $"{HttpMethod} {Endpoint}" : Endpoint;

        public override bool Equals(object obj)
            => Equals(obj as BucketId);

        public override int GetHashCode()
            =>  IsHashBucket ? (BucketHash, string.Join("/", MajorParams.Select(x => x.Value))).GetHashCode() : (HttpMethod, Endpoint).GetHashCode();

        public override string ToString()
            => GetBucketHash() ?? GetUniqueEndpoint();

        public bool Equals(BucketId other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (GetType() != other.GetType())
                return false;
            return ToString() == other.ToString();
        }        
    }
}
