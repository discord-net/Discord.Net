using System.Diagnostics;

namespace Discord
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class StreamingGame : Game
    {
        public string Url { get; internal set; }
        public StreamType StreamType { get; internal set; }

        public StreamingGame(string name, string url, StreamType streamType)
        {
            Name = name;
            Url = url;
            StreamType = streamType;
        }
        
        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Url})";
    }
}