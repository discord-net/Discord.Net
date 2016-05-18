namespace Discord
{
    public struct Game
    {
        public string Name { get; }
        public string Url { get; }
        public GameType Type { get; }

        public Game(string name)
        {
            Name = name;
            Url = null;
            Type = GameType.Default;
        }
        public Game(string name, GameType type, string url)
        {
            Name = name;
            Url = url;
            Type = type;
        }
    }
}
