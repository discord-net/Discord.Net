using System.Diagnostics;

namespace Discord
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public struct Game
    {
        public string Name { get; }
        public string StreamUrl { get; }
        public StreamType StreamType { get; }

        public Game(string name, string streamUrl, StreamType type)
        {
            Name = name;
            StreamUrl = streamUrl;
            StreamType = type;
        }
        private Game(string name)
            : this(name, null, StreamType.NotStreaming) { }

        public override string ToString() => Name;
        private string DebuggerDisplay => StreamUrl != null ? $"{Name} ({StreamUrl})" : Name;
    }
}
