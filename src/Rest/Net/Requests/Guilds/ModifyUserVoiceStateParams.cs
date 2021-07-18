namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record ModifyUserVoiceStateParams
    {
        /// <summary>
        /// The id of the channel the user is currently in.
        /// </summary>
        public Snowflake ChannelId { get; set; }

        /// <summary>
        /// Toggles the user's suppress state.
        /// </summary>
        public Optional<bool> Suppress { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
        }
    }
}
