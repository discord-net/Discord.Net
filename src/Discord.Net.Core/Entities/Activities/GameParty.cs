namespace Discord
{
    /// <summary>
    ///     Party information for a <see cref="RichGame" /> object.
    /// </summary>
    public class GameParty
    {
        internal GameParty() { }

        /// <summary>
        ///     Gets the ID of the party.
        /// </summary>
        public string Id { get; internal set; }
        public long Members { get; internal set; }
        /// <summary>
        ///     Gets the party's current and maximum size.
        /// </summary>
        public long Capacity { get; internal set; }
    }
}
