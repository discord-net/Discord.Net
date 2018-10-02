namespace Discord
{
    /// <summary>
    ///     Party secret for a <see cref="RichGame" /> object.
    /// </summary>
    public class GameSecrets
    {
        /// <summary>
        ///     Gets the secret for a specific instanced match.
        /// </summary>
        public string Match { get; }
        /// <summary>
        ///     Gets the secret for joining a party.
        /// </summary>
        public string Join { get; }
        /// <summary>
        /// 	Gets the secret for spectating a game.
        /// </summary>
        public string Spectate { get; }

        internal GameSecrets(string match, string join, string spectate)
        {
            Match = match;
            Join = join;
            Spectate = spectate;
        }
    }
}
