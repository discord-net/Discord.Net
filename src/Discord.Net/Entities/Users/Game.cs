using Model = Discord.API.Game;

namespace Discord
{
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
        public Game(string name)
            : this(name, null, StreamType.NotStreaming) { }
        internal Game(Model model)
            : this(model.Name, model.StreamUrl, model.StreamType ?? StreamType.NotStreaming) { }
    }
}
