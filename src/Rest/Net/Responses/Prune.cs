namespace Discord.Net.Rest
{
    /// <summary>
    /// Response to count or begin a prune.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/guild#get-guild-prune-count"/> or
    /// <see href="https://discord.com/developers/docs/resources/guild#begin-guild-prune"/>
    /// </remarks>
    public record Prune
    {
        /// <summary>
        /// Gets the number of members that would or were affected in a prune operation.
        /// </summary>
        public Optional<int> Pruned { get; init; }
    }
}
