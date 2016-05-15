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
    }
}
