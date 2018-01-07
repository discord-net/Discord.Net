namespace Discord
{
    public class GameSecrets
    {
        public string Match { get; }
        public string Join { get; }
        public string Spectate { get; }

        internal GameSecrets(string match, string join, string spectate)
        {
            Match = match;
            Join = join;
            Spectate = spectate;
        }
    }
}