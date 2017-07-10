namespace Discord
{
    public struct ReactionMetadata
    {
        /// <summary> Gets the number of reactions </summary>
        public int ReactionCount { get; internal set; }

        /// <summary> Returns true if the current user has used this reaction </summary>
        public bool IsMe { get; internal set; }
    }
}
