namespace Discord
{
    /// <summary>
    ///     A metadata containing reaction information.
    /// </summary>
    public struct ReactionMetadata
    {
        /// <summary>
        ///     Gets the number of reactions.
        /// </summary>
        public int ReactionCount { get; internal set; }

        /// <summary>
        ///     Returns <c>true</c> if the current user has used this reaction.
        /// </summary>
        public bool IsMe { get; internal set; }
    }
}
