using System.Diagnostics;
using Model = Discord.API.Game;

namespace Discord
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class Game
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
        public Game(string name)
            : this(name, null, StreamType.NotStreaming) { }
        internal Game(Model model)
            : this(model.Name, model.StreamUrl.GetValueOrDefault(null), model.StreamType.GetValueOrDefault(null) ?? StreamType.NotStreaming) { }

        public override string ToString() => Name;
        private string DebuggerDisplay => StreamUrl != null ? $"{Name} ({StreamUrl})" : Name;
    }
}
