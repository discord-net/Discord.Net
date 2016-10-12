using System.Collections.Immutable;

namespace Discord.Net.Queue
{
    public struct ClientBucket
    {
        private static readonly ImmutableDictionary<string, ClientBucket> _defs;
        static ClientBucket()
        {
            var builder = ImmutableDictionary.CreateBuilder<string, ClientBucket>();
            builder.Add("<test>", new ClientBucket(5, 5));
            _defs = builder.ToImmutable();
        }

        public static ClientBucket Get(string id) => _defs[id];

        public int WindowCount { get; }
        public int WindowSeconds { get; }

        public ClientBucket(int count, int seconds)
        {
            WindowCount = count;
            WindowSeconds = seconds;
        }
    }
}
