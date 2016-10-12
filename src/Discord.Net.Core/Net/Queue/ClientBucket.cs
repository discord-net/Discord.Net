using System.Collections.Immutable;

namespace Discord.Net.Queue
{
    public struct ClientBucket
    {
        public const string SendEditId = "<send_edit>";

        private static readonly ImmutableDictionary<string, ClientBucket> _defs;
        static ClientBucket()
        {
            var builder = ImmutableDictionary.CreateBuilder<string, ClientBucket>();
            builder.Add(SendEditId, new ClientBucket(10, 10));
            _defs = builder.ToImmutable();
        }

        public static ClientBucket Get(string id) =>_defs[id];

        public int WindowCount { get; }
        public int WindowSeconds { get; }

        public ClientBucket(int count, int seconds)
        {
            WindowCount = count;
            WindowSeconds = seconds;
        }
    }
}
