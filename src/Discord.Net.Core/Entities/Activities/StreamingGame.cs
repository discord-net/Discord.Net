using System.Diagnostics;

namespace Discord
{
    [DebuggerDisplay(@"{" + nameof(DebuggerDisplay) + @",nq}")]
    public class StreamingGame : Game
    {
        public StreamingGame(string name, string url)
        {
            Name = name;
            Url = url;
            Type = ActivityType.Streaming;
        }

        public string Url { get; internal set; }
        private string DebuggerDisplay => $"{Name} ({Url})";

        public override string ToString() => Name;
    }
}
