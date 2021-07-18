namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record GetGuildParams
    {
        /// <summary>
        /// When true, will return approximate member and presence counts for the guild.
        /// </summary>
        public Optional<bool> WithCounts { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
        }
    }
}
