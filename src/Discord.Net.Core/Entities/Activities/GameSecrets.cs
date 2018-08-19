namespace Discord
{
    public class GameSecrets
    {
        internal GameSecrets(string match, string join, string spectate)
        {
            Match = match;
            Join = join;
            Spectate = spectate;
        }

        public string Match { get; }
        public string Join { get; }
        public string Spectate { get; }
    }
}
