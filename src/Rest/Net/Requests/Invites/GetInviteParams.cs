namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record GetInviteParams
    {
        /// <summary>
        /// Whether the invite should contain approximate member counts.
        /// </summary>
        public Optional<bool> WithCounts { get; set; }

        /// <summary>
        /// Whether the invite should contain the expiration date.
        /// </summary>
        public Optional<bool> WithExpiration { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
        }
    }
}
