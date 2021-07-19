namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record CreateDMParams
    {
        /// <summary>
        /// The recipient to open a DM channel with.
        /// </summary>
        public Snowflake RecipientId { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
            Preconditions.NotZero(RecipientId, nameof(RecipientId));
        }
    }
}
