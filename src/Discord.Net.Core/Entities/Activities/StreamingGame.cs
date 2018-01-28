using System.Diagnostics;

namespace Discord
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class StreamingGame : Game
    {
        public string Url { get; internal set; }

        public StreamingGame(string name, string url)
        {
            Name = name;
            Url = url;
            Type = ActivityType.Streaming;
        }

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Url})";
    }
}